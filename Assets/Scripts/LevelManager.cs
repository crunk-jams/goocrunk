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

	[SerializeField] private CanvasGroup assimilation = null;
	[SerializeField] private float maxAssimilation = 0.5f;

	private void Start()
	{
		loadedLevel = FindObjectOfType<Level>();
		BeginLevel();
	}

	public void BeginLevel(bool newLevel = false)
	{
		if (!newLevel && loadedLevel?.startingPath != null)
		{
			loadedLevel.startingPath.GiveDirection(player.transform);
		}

		if (loadedLevel != null)
		{
			Destroy(loadedLevel.gameObject);
		}

		loadedLevel = Instantiate(levels[currentLevel - 1], levelAnchor);
		loadedLevel.transform.localPosition = Vector3.zero;
		loadedLevel.transform.localRotation = Quaternion.identity;


		player.transform.position = Vector3.zero;
		// Arduino change ... keep pointing where the gun is
		//player.transform.rotation = Quaternion.identity;
		player.grounded = 0;
		// Arduino change ... keep pointing where the gun is
		//cam.transform.rotation = Quaternion.identity;
		AudioManager.Instance.ChangedLevel(currentLevel);
	}

	public void NextLevel()
	{
		if (currentLevel >= levels.Length)
		{
			Cursor.lockState = CursorLockMode.None;
			SceneManager.LoadScene(endGameScreen);
			return;
		}

		currentLevel++;

		if (assimilation != null)
		{
			assimilation.alpha = (((float)currentLevel) / (levels.Length)) * maxAssimilation;
		}

		BeginLevel(true);

		var arrows = FindObjectsOfType<Arrow>();
		for (int i = arrows.Length - 1; i >= 0; i--)
		{
			if (arrows[i] != null)
			{
				Destroy(arrows[i].gameObject);
			}
		}
	}
}
