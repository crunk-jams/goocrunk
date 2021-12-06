using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
	[SerializeField] private string nextScene = "MainMenu";
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<Runner>() != null)
		{
			SceneManager.LoadScene(nextScene);
		}
	}
}
