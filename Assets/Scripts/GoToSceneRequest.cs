using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToSceneRequest : MonoBehaviour
{
	public string sceneName = null;

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
