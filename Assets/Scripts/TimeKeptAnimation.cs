using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeptAnimation : MonoBehaviour
{
	private float cachedTimeScale = 1;
	[SerializeField] private Animator anim = null;
	private TimeKeeper timeKeeper = null;

	private void Start()
	{
		timeKeeper = FindObjectOfType<TimeKeeper>();
	}

	private void Update()
	{
		if (cachedTimeScale != timeKeeper.TimeScale)
		{
			cachedTimeScale = timeKeeper.TimeScale;
			anim.speed = timeKeeper.TimeScale;
		}
	}
}
