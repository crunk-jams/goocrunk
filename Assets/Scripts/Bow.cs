using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
	[SerializeField] private Rigidbody body = null;
	[SerializeField] private Rigidbody arrowPrefab = null;
	[SerializeField] private Transform arrowContainer = null;
	[SerializeField] private Reticle reticle;
	[SerializeField] private float minShotForce = 10;
	[SerializeField] private float maxShotForce = 50;
	[SerializeField] private float minPull = 0f;
	[SerializeField] private float maxPull = 0.5f;
	[SerializeField] private float requiredPullBack = 0.1f;
	[SerializeField] private float screenPortionForMax = 0.5f;

	private bool nocked = false;
	private Vector2 nockStartPos = Vector2.zero;
	private Rigidbody arrow = null;
	private bool waitingPostShot = false;
	private float cachedInputY = 0;

	private void Update()
	{
		var inputPos = Input.mousePosition;
		bool attemptingToNock = Input.GetMouseButton(0);

		// Skip post shot wait if the player wants to nock an arrow or move camera down,
		// because the player is not trying to move the camera back to pre-nock position.
		if (attemptingToNock || inputPos.y < cachedInputY)
		{
			waitingPostShot = false;
		}

		if (arrow == null && !waitingPostShot)
		{
			arrow = Instantiate(arrowPrefab, arrowContainer);
			arrow.transform.localPosition = Vector3.zero;
		}

		var pullback = Mathf.Clamp01((nockStartPos.y - inputPos.y) / (Screen.height * screenPortionForMax));

			var pullDistance = minPull;

			if (nocked)
			{
				pullDistance = Mathf.Lerp(minPull, maxPull, pullback);
			}

			if (arrow != null)
			{
				var arrowLocalScale = arrow.transform.localScale;
				arrowLocalScale.z = pullDistance;
				arrow.transform.localScale = arrowLocalScale;
				arrow.transform.localRotation = Quaternion.identity;
			}

			if (attemptingToNock)
			{
				if (!nocked && arrow != null)
				{
					nocked = true;
					nockStartPos = inputPos;
					reticle.Lock();
					arrow.transform.forward = transform.forward;
				}
			}
			else if (nocked)
			{
				if (pullback < requiredPullBack)
				{
					if (arrow != null)
					{
						Destroy(arrow.gameObject);
						arrow = null;
					}
				}

				if (arrow != null)
				{
					var shotForce = Mathf.Lerp(minShotForce, maxShotForce, pullback);
					arrow.transform.parent = null;

					arrow.transform.forward = transform.forward;
					arrow.isKinematic = false;
					arrow.velocity = body.velocity;
					arrow.AddForce(transform.forward * shotForce, ForceMode.Impulse);
					arrow = null;
				}

				nocked = false;
				StartCoroutine(RecoverAfterShot());
			}

			cachedInputY = inputPos.y;
	}

	private IEnumerator RecoverAfterShot()
	{
		reticle.StartRecover();

		waitingPostShot = true;
		while (Input.mousePosition.y < nockStartPos.y && waitingPostShot)
		{
			yield return null;
		}
		waitingPostShot = false;

		reticle.EndRecover();
	}
}
