using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEnforcer : MonoBehaviour
{
	private void Awake()
	{
		if (FindObjectOfType<MenuProof>() == null)
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
