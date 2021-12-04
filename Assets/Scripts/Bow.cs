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
	[SerializeField] private float postShotDelay = 0.5f;
	[SerializeField] private float screenPortionForMax = 0.5f;

	private bool nocked = false;
	private Vector2 nockStartPos = Vector2.zero;
	private Rigidbody arrow = null;
	private bool waitingPostShot = false;


	private void Update()
	{
		var inputPos = Input.mousePosition;

		if (!waitingPostShot)
		{
			if (arrow == null && !waitingPostShot)
			{
				arrow = Instantiate(arrowPrefab, arrowContainer);
			}

			var pullback = Mathf.Clamp01(-(inputPos.y - nockStartPos.y) / (Screen.height * screenPortionForMax));

			if (nocked)
			{
				var pullDistance = Mathf.Lerp(minPull, maxPull, pullback);
				var arrowLocalPos = arrow.transform.localPosition;
				arrowLocalPos.z = -pullDistance;
				arrow.transform.localPosition = arrowLocalPos;
			}

			if (Input.GetMouseButton(0))
			{
				if (!nocked)
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

					var toReticle = (reticle.transform.position - transform.position).normalized;

					arrow.transform.forward = toReticle;
					arrow.isKinematic = false;
					arrow.velocity = body.velocity;
					arrow.AddForce(toReticle * shotForce, ForceMode.Impulse);
					arrow = null;
				}
				nocked = false;
				StartCoroutine(WaitAfterShot());
			}
		}


	}

	private IEnumerator WaitAfterShot()
	{
		waitingPostShot = true;
		yield return new WaitForSeconds(postShotDelay);
		waitingPostShot = false;

		reticle.Unlock();
	}
}
