using UnityEngine;

public sealed class Pause : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private LineRenderer lineRenderer;

	private void Awake() => SetPaused(false);

	public void OnPause()
	{
		SetPaused(true);
		lineRenderer.enabled = false;
	} 

	public void Resume()
	{
		SetPaused(false);
		lineRenderer.enabled = true;
	} 

	private void SetPaused(bool paused)
	{
		Time.timeScale = paused ? 0f : 1f;
		pauseMenu.SetActive(paused);
		Cursor.visible = paused;
		Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Confined;
	}

	private void OnDisable() => Time.timeScale = 1f;
}