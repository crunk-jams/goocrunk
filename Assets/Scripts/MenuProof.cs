using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuProof : MonoBehaviour
{
	private static MenuProof instance = null;
	public static MenuProof Instance => instance;
	public Texture2D cursorImage = null;
	private List<GameObject> sceneSurvivors = new();

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		if (cursorImage != null)
		{
			Cursor.SetCursor(cursorImage, new Vector2(cursorImage.width, cursorImage.height), CursorMode.Auto);
		}

		if (instance == null)
		{
			instance = this;
		}
		else
		{
			instance.NoSurvivors();
			Destroy(gameObject);
			return;
		}

		Application.targetFrameRate = 60;
		DontDestroyOnLoad(gameObject);

		var sceneRequest = FindObjectOfType<GoToSceneRequest>();
		if (sceneRequest != null)
		{
			FindObjectOfType<GotoScene>().scene = sceneRequest.sceneName;
		}
	}

	public void OutLiveScene(GameObject survivor)
	{
		DontDestroyOnLoad(survivor);
		sceneSurvivors.Add(survivor);
	}

	public void NoSurvivors()
	{
		foreach (var survivor in sceneSurvivors)
		{
			Destroy(survivor);
		}
		sceneSurvivors.Clear();
	}
}
