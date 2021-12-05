using System;
using UnityEngine;

public class DirectionTrigger : MonoBehaviour
{
	[SerializeField] private Transform endDirection = null;
	[SerializeField] private float turnDuration = 0.2f;
	private PlayerCamera playerCamera = null;

	private void Awake()
	{
		playerCamera = FindObjectOfType<PlayerCamera>();
	}

	public void GiveDirection(Transform target)
	{
		if (target != null && target == playerCamera.Player.transform)
		{
			target.LookAt(target.position + endDirection.forward, endDirection.up);

			// Hack to turn player instantly when using TeleTargets
			if (turnDuration < 0.001)
			{
				playerCamera.transform.rotation = target.rotation;
			}
		}
	}

	public void ForeshadowDirection(Transform target)
	{
		if (target != null && target == playerCamera.Player.transform)
		{
			playerCamera.TurnTowards(endDirection.forward, endDirection.up, turnDuration);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		ForeshadowDirection(other.transform);
	}

	private void OnTriggerExit(Collider other)
	{
		GiveDirection(other.transform);
	}
}
