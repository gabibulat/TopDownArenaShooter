using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMusicPlayer : MonoBehaviour
{
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AssetReferenceT<AudioClip> music;
	private AsyncOperationHandle<AudioClip>? _currentHandle;

	private void Start()
	{
		PlayMusic(music);
	}

	private void PlayMusic(AssetReferenceT<AudioClip> clip)
	{
		if (_currentHandle.HasValue && _currentHandle.Value.IsValid())
			Addressables.Release(_currentHandle.Value);

		var handle = Addressables.LoadAssetAsync<AudioClip>(clip);
		_currentHandle = handle;

		handle.Completed += result =>
		{
			if (result.Status != AsyncOperationStatus.Succeeded || !result.Result) return;

			musicSource.clip = result.Result;
			musicSource.loop = true;
			musicSource.Play();
		};
	}

	private void OnDestroy()
	{
		if (_currentHandle.HasValue && _currentHandle.Value.IsValid())
			Addressables.Release(_currentHandle.Value);
	}
}