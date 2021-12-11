using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeptBody : MonoBehaviour
{
	private float cachedTimeScale = 1;
	[SerializeField] private Rigidbody body = null;
	private TimeKeeper timeKeeper = null;

	private void Start()
	{
		timeKeeper = FindObjectOfType<TimeKeeper>();
	}

	private void LateUpdate()
	{
		//if (cachedTimeScale != timeKeeper.TimeScale)
		//{
			cachedTimeScale = timeKeeper.TimeScale;
			body.velocity *= timeKeeper.TimeScale;

			if (!timeKeeper.HasFocus)
			{
				body.velocity = Vector3.zero;
			}
			//}
	}
}
