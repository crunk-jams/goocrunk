using System;
using UnityEngine;

public class DirectionTrigger : MonoBehaviour
{
	[SerializeField] private Transform endDirection = null;
	[SerializeField] private float turnDuration = 0.2f;
	[SerializeField] private float pathWidth = 1;
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
			var runner = target.GetComponent<Runner>();
			if (runner != null)
			{
				runner.SetPathStats(transform.position, endDirection.forward, pathWidth);
			}

			// Hack to turn player instantly when using TeleTargets
			if (turnDuration < 0.001)
			{
				playerCamera.transform.rotation = target.rotation;
			}
		}
	}

	/*private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(endDirection.transform.position, new Vector3(pathWidth, 1, 3));
		//pathBoxGizmo.DefineBox(endDirection.transform.position, new Vector3(pathWidth, 1, 3), Color.blue);
	}*/

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
