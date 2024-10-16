using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bow : MonoBehaviour
{
	[SerializeField] private Animator anim = null;
	[SerializeField] private Rigidbody body = null;
	[SerializeField] private Rigidbody nockedArrowPrefab = null;
	[SerializeField] private Rigidbody shotArrowPrefab = null;

	[SerializeField] private Transform arrowContainer = null;
	[SerializeField] private Reticle reticle;
	[SerializeField] private float minShotForce = 10;
	[SerializeField] private float maxShotForce = 50;
	[SerializeField] private float baseArrowSize = 0.05f;
	[SerializeField] private float minPull = 0f;
	[SerializeField] private float maxPull = 0.5f;
	[SerializeField] private float requiredPullBack = 0.1f;
	[SerializeField] private float screenPortionForMax = 0.5f;
	[SerializeField] private float gooTimeScale = 0.5f;

	private bool nocked = false;
	private Vector2 nockStartPos = Vector2.zero;
	private Vector2 oldMousePos = Vector2.zero;
	private Rigidbody arrow = null;
	private bool waitingPostShot = false;

	private void Awake()
	{
		oldMousePos = Input.mousePosition;
	}

	private void Update()
	{
		var inputPos = oldMousePos + (new Vector2(
			Input.GetAxis("Mouse X") * Screen.width ,
			Input.GetAxis("Mouse Y") * Screen.height)); //Input.mousePosition;
		oldMousePos = inputPos;

		// Arduino changes
		//bool attemptingToNock = Input.GetMouseButton(0);
		bool attemptingToNock = Input.GetKey(KeyCode.F) || nocked;

		if (arrow == null && attemptingToNock)
		{
			arrow = Instantiate(nockedArrowPrefab, arrowContainer);
			arrow.gameObject.SetActive(true);
			arrow.transform.localPosition = Vector3.zero;
			reticle.pullStrength = 0f;
		}

		// Arduino changes
		// If the player pulls the mouse above where they starting nocking the arrow, start measuring from the new position.
		//if (inputPos.y > nockStartPos.y)
		//{
		//	nockStartPos.y = inputPos.y;
		//}

		//var pullback = Mathf.Clamp01((nockStartPos.y - inputPos.y) / (Screen.height * screenPortionForMax));
		var pullback =
			Input.GetKey(KeyCode.Z)   ? 1.00f
			: Input.GetKey(KeyCode.Y) ? 0.50f
			: Input.GetKey(KeyCode.X) ? 0.25f
			: Input.GetKey(KeyCode.W) ? 0.05f
			: 0;

		// TODO @Sam do we need this, or can we just check if any of these keys are pressed.
		(pullback, attemptingToNock) =
			Input.GetKey(KeyCode.D)   ? (Mathf.Min(1.00f, pullback), false)
			: Input.GetKey(KeyCode.C) ? (Mathf.Min(0.50f, pullback), false)
			: Input.GetKey(KeyCode.B) ? (Mathf.Min(0.25f, pullback), false)
			: Input.GetKey(KeyCode.A) ? (Mathf.Min(0.05f, pullback), false)
			: (pullback, attemptingToNock);

		pullback = Mathf.Max(pullback, reticle.pullStrength);

		reticle.pullStrength = pullback;

		reticle.shotReady = pullback >= requiredPullBack;

		var pullDistance = minPull;

		if (nocked)
		{
			pullDistance = Mathf.Lerp(minPull, maxPull, pullback);
		}

		if (arrow != null)
		{
			var arrowLocalScale = arrow.transform.localScale;
			var arrowSize = baseArrowSize + ((1 - baseArrowSize) * pullDistance);
			arrowLocalScale.z = arrowSize;
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
			AudioManager.Instance.ChargeShot(pullback);
			Time.timeScale = gooTimeScale;
		}
		else
		{
			Time.timeScale = 1;

			if (nocked)
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
					if (Random.Range(0f, 1f) < 0.5f)
					{
						anim.SetTrigger("Shoot");
					}
					else
					{
						anim.SetTrigger("Shoot2");
					}

					var oldArrow = arrow.transform;
					arrow = Instantiate(shotArrowPrefab, arrowContainer);
					arrow.gameObject.SetActive(true);
					arrow.transform.localPosition = oldArrow.localPosition;
					arrow.transform.localRotation = oldArrow.localRotation;
					arrow.transform.localScale = oldArrow.localScale;
					arrow.transform.parent = null;
					Destroy(oldArrow.gameObject);

					var shotForce = Mathf.Lerp(minShotForce, maxShotForce, pullback);

					var toReticle = (reticle.transform.position - transform.position).normalized;
					arrow.transform.forward = toReticle;
					arrow.isKinematic = false;
					arrow.velocity = body.velocity;
					arrow.AddForce(toReticle * shotForce, ForceMode.Impulse);
					arrow = null;
				}

				nocked = false;
				reticle.Unlock();
				reticle.pullStrength = 0;
				AudioManager.Instance.FireShot(pullback);
			}
		}
	}
}
