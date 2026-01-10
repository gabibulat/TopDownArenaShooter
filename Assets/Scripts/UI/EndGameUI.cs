using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class EndGameUI : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;
	[SerializeField] private GameController gameController;
	[SerializeField] private TMP_Text endGameText;
	[SerializeField] private GameObject endGameWindow;
	[SerializeField] private LocalizedString gameOverString;
	[SerializeField] private LocalizedString winString;

	private LocalizedString _currentString;

	private void OnEnable()
	{
		playerController.PlayerDied += ShowGameOver;
		gameController.Won += ShowWin;
	}

	private void ShowGameOver() => Show(gameOverString);

	private void ShowWin() => Show(winString);

	private void Show(LocalizedString localizedString)
	{
		UnsubscribeCurrent();
		_currentString = localizedString;
		_currentString.StringChanged += OnTextChanged;
		_currentString.RefreshString();
		endGameWindow.SetActive(true);
	}

	private void OnTextChanged(string value) => endGameText.text = value;

	private void UnsubscribeCurrent()
	{
		if (_currentString != null) _currentString.StringChanged -= OnTextChanged;
	}

	private void OnDisable()
	{
		playerController.PlayerDied -= ShowGameOver;
		UnsubscribeCurrent();
	}
}