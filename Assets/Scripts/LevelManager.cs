using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private string menuScene = "MainMenu";
	[SerializeField] private string endGameScreen = "MainMenu";

	[SerializeField] private Transform levelAnchor = null;
	[SerializeField] private Level[] levels;
	private Level loadedLevel = null;
	[SerializeField] private Runner player = null;
	[SerializeField] private PlayerCamera cam = null;

	[SerializeField] private int currentLevel = 1;

	private void Start()
	{
		loadedLevel = FindObjectOfType<Level>();
		BeginLevel();
	}

	public void BeginLevel()
	{
		if (loadedLevel != null)
		{
			Destroy(loadedLevel.gameObject);
		}

		loadedLevel = Instantiate(levels[currentLevel - 1], levelAnchor);
		loadedLevel.transform.localPosition = Vector3.zero;
		loadedLevel.transform.localRotation = Quaternion.identity;

		player.transform.position = Vector3.zero;
		player.transform.rotation = Quaternion.identity;
		player.grounded = 0;
		cam.transform.rotation = Quaternion.identity;
		AudioManager.Instance.ChangedLevel(currentLevel);
	}

	public void NextLevel()
	{
		if (currentLevel >= levels.Length)
		{
			SceneManager.LoadScene(endGameScreen);
			return;
		}

		currentLevel++;

		BeginLevel();
	}
}
