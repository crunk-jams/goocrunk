using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuProof : MonoBehaviour
{
	private static MenuProof instance = null;

	private void Awake()
	{
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
