using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuProof : MonoBehaviour
{
	private static MenuProof instance = null;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// TODO Destroy Everything that is already DontDestroyOnLoad so we don't get duplicates
		// maybe when instance changes
		if (instance == null)
		{
			instance = this;
		}
		else
		{
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
}
