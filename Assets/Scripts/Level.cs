using System;
using UnityEngine;

public class Level : MonoBehaviour
{
	[SerializeField] private DirectionTrigger startingPath = null;

	private void Start()
	{
		if (startingPath != null)
		{
			startingPath.GiveDirection(FindObjectOfType<Runner>().transform);
		}
	}
}
