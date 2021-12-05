using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoScene : MonoBehaviour
{
	public string scene = null;

	public void Event_GotoScene()
	{
		if (string.IsNullOrEmpty(scene))
		{
			scene = SceneManager.GetActiveScene().name;
		}

		SceneManager.LoadScene(scene);
	}
}
