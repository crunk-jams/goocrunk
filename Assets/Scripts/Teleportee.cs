using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportee : MonoBehaviour
{
	[SerializeField] private float teleportDuration = 0;
	private Coroutine teleporting = null;
	[SerializeField] private Rigidbody body = null;
	[SerializeField] private Collider[] colliders = null;
	[SerializeField] private Runner runner = null;

	public void TeleportTo(Transform target)
	{
		if (teleporting == null)
		{
			if (teleportDuration <= 0)
			{
				transform.position = target.transform.position;
			}
			else
			{
				teleporting = StartCoroutine(teleportOverTime(target));
			}
		}
	}

	private IEnumerator teleportOverTime(Transform target)
	{
		var targetPosition = target.position;
		var toTarget = targetPosition - transform.position;
		var toTargetMag = toTarget.magnitude;
		var toTargetDir = toTarget / toTargetMag;
		var teleportSpeed = (toTargetMag / teleportDuration) * Time.deltaTime;

		runner.enabled = false;
		runner.grounded = 0;
		body.isKinematic = true;
		foreach (var col in colliders)
		{
			col.enabled = false;
		}

		while (true)
		{
			if ((transform.position - targetPosition).sqrMagnitude < teleportSpeed * teleportSpeed)
			{
				transform.position = targetPosition;
				break;
			}
			else
			{
				transform.position += teleportSpeed * toTargetDir;
			}

			yield return null;
		}

		runner.enabled = true;
		body.isKinematic = false;
		foreach (var col in colliders)
		{
			col.enabled = true; // TODO sure hope we don't turn on one that was supposed to be off
		}

		teleporting = null;
	}
}
