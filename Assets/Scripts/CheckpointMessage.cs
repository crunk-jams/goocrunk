using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMessage : MonoBehaviour
{
	public string level = String.Empty;
	public Vector3 position = Vector3.zero;
	public Quaternion rotation = Quaternion.identity;
	public float speedIntensity = 0;
	public Vector3 pathStart = Vector3.zero;
	public Vector3 pathDirection = Vector3.zero;
	public float pathWidth = 0;


	private void Awake()
	{
		DontDestroyOnLoad(this);
		gameObject.name = "CheckpointMessage";
	}
}
