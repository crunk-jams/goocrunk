using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<Runner>() != null)
		{
			FindObjectOfType<LevelManager>().NextLevel();
			PlayerLives.Instance.GainLife();
			AudioManager.Instance.HitCheckPoint();
		}
	}
}
