using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoScene : MonoBehaviour
{
	public string scene = null;

	public void Event_GotoScene()
	{
		SceneManager.LoadScene(scene);
	}
}
