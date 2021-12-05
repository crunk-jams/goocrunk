using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEnforcer : MonoBehaviour
{
	[SerializeField] private GoToSceneRequest requestReturnPrefab;

	private void Awake()
	{
		if (requestReturnPrefab != null)
		{
			var request = Instantiate(requestReturnPrefab);
			request.sceneName = SceneManager.GetActiveScene().name;
		}

		if (FindObjectOfType<MenuProof>() == null)
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
