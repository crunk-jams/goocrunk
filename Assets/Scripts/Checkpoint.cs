using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var runner = other.GetComponent<Runner>();
		if (runner != null)
		{
			runner.checkpoint = this;
		}
	}
}
