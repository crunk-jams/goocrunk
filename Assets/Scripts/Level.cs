using System;
using UnityEngine;

public class Level : MonoBehaviour
{
	public DirectionTrigger startingPath = null;

	private void Start()
	{
		var runner = FindObjectOfType<Runner>();
		if (startingPath != null)
		{
			startingPath.GiveDirection(runner.transform);
		}
	}

	// TODO @Sam setup P key to Pause or Reset level (that is wired in arduino)
}
