using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuProof : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		var sceneRequest = FindObjectOfType<GoToSceneRequest>();
		if (sceneRequest != null)
		{
			FindObjectOfType<GotoScene>().scene = sceneRequest.sceneName;
		}
	}
}
