using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField] private Transform player = null;
	public Transform Player => player;
	[SerializeField] private Vector3 offset = Vector3.zero;

	private bool turning = false;
	private Vector3 desiredTurn = Vector3.zero;
	private float turnDuration = 1;
	private float turnElapsed = 0;

	private void Start()
	{
		offset = transform.position - player.transform.position;
	}

	private void Update()
	{
		transform.position = player.transform.position + offset;

		if (turning)
		{
			var dt = Time.deltaTime;
			var remainingTurn = turnDuration - turnElapsed;
			turnElapsed += dt;
			if (dt > remainingTurn)
			{
				dt = remainingTurn;
				turning = false;
			}

			transform.Rotate((desiredTurn * dt) / turnDuration, Space.Self);

			if (!turning)
			{
				turnDuration = 0;
				turnElapsed = 0;
			}
		}
	}

	public void TurnTowards(Vector3 finalForward, Vector3 finalUp, float duration)
	{
		var rotToForward = Mathf.Acos(Vector3.Dot(finalForward.normalized, transform.forward));
		if (Vector3.Dot(finalForward, transform.right) < 0)
		{
			rotToForward *= -1;
		}

		var rotToUp = Mathf.Acos(Vector3.Dot(finalUp.normalized, transform.up));
		if (Vector3.Dot(finalUp, -transform.right) < 0)
		{
			rotToUp *= -1;
		}

		desiredTurn = new Vector3(0, rotToForward, rotToUp) * Mathf.Rad2Deg;
		turnDuration = duration;
		turnElapsed = 0;
		turning = true;
	}
}
