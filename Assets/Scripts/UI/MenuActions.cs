using UnityEngine;

public class MenuActions : MonoBehaviour
{
	public void BackToMainMenu()
	{
		if (LevelLoader.Instance) LevelLoader.Instance.LoadMainMenu();
	}
	
	public void LoadLevel(int levelIndex)
	{
		LevelLoader.Instance.LoadLevelByIndex(levelIndex);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}