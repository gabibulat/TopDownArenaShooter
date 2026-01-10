using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public sealed class LevelLoader : MonoBehaviour
{
	public static LevelLoader Instance { get; private set; }
	[SerializeField] private LevelCatalog catalog;
	[SerializeField] private AssetReference mainMenuScene;
	
	private AsyncOperationHandle<LevelData>? _levelHandle;
	private LevelData _currentLevelData;
	private int _currentIndex = -1;
	private string _currentSceneKey;
	private AsyncOperationHandle<SceneInstance>? _sceneHandle;

	public event Action<LevelData> LevelReady;
	
	private void Awake()
	{
		if (Instance && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void LoadLevelByIndex(int index)
	{
		_ = LoadLevelInternal(catalog.Clamp(index));
	}

	public bool LoadNextLevel()
	{
		if (!catalog || catalog.Count == 0) return false;
		var next = _currentIndex + 1;
		if (next >= catalog.Count) return false;
		_ = LoadLevelInternal(next);
		return true;
	}

	private async Task LoadLevelInternal(int index)
	{
		_currentIndex = index;
		
		ReleaseCurrentLevelData();
		
		var levelRef = catalog.levels[index];
		var level = await LoadLevelData(levelRef);
		if (!level) return;
		
		_currentLevelData = level;

		var desiredSceneKey = level.levelScene.RuntimeKey.ToString();
		if (string.IsNullOrEmpty(_currentSceneKey) || _currentSceneKey != desiredSceneKey)
		{
			await LoadSceneSingle(level.levelScene);
			_currentSceneKey = desiredSceneKey;
		}

		LevelReady?.Invoke(_currentLevelData);
	}
	
	private void ReleaseCurrentLevelData()
	{
		if (!_levelHandle.HasValue) return;
		Addressables.Release(_levelHandle.Value);
		_levelHandle = null;
	}

	private async Task<LevelData> LoadLevelData(AssetReferenceT<LevelData> levelRef)
	{
		_levelHandle = levelRef.LoadAssetAsync();
		await _levelHandle.Value.Task;
		if (_levelHandle.Value.Status == AsyncOperationStatus.Succeeded) return _levelHandle.Value.Result;
		ReleaseCurrentLevelData();
		return null;
	}

	private async Task LoadSceneSingle(AssetReference sceneRef)
	{
		_sceneHandle = sceneRef.LoadSceneAsync(LoadSceneMode.Single, activateOnLoad: true);
		await _sceneHandle.Value.Task;
		if (_sceneHandle.Value.Status == AsyncOperationStatus.Succeeded)
		{
			SceneManager.SetActiveScene(_sceneHandle.Value.Result.Scene);
		}
	}
	
	public void LoadMainMenu()
	{
		if (mainMenuScene == null) return;
		_ = LoadMainMenuInternal();
	}

	private async Task LoadMainMenuInternal()
	{
		ReleaseCurrentLevelData();
		var menuKey = mainMenuScene.RuntimeKey.ToString();
		if (!string.IsNullOrEmpty(_currentSceneKey) && _currentSceneKey == menuKey) return;
		_currentIndex = -1;
		_currentLevelData = null;
		await LoadSceneSingle(mainMenuScene);
		_currentSceneKey = menuKey;
	}

	private void OnDestroy()
	{
		ReleaseCurrentLevelData();

		if (Instance == this)
			Instance = null;
	}
}