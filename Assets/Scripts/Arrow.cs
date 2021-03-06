using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	[SerializeField] private Animator anim = null;
	[SerializeField] private string hitParam = "Hit";

	private Rigidbody body = null;
	private Collider bodyCollider = null;

	private Vector3 cachedPos = new Vector3(0, -1000, 0);

	[SerializeField] private bool testApex;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		bodyCollider = GetComponent<Collider>();
		bodyCollider.enabled = false;
	}


	private void Update()
	{
		var loosed = body != null && body.useGravity && !body.isKinematic;
		if (loosed && body.velocity.sqrMagnitude > 0)
		{
			transform.forward = body.velocity.normalized;

			// Test apex
			#if UNITY_EDITOR
			if (testApex)
			{
				if (transform.position.y < cachedPos.y)
				{
					Debug.Break();
					transform.transform.position = cachedPos;
					body.isKinematic = true;
					cachedPos.y = -1000;
				}
				else
				{
					cachedPos = transform.position;
				}
			}
			#endif
		}

		if (bodyCollider != null)
		{
			bodyCollider.enabled = loosed;
		}
	}

	public void SuccessfulHit()
	{
		transform.localScale *= 2;
	}

	private void OnCollisionEnter(Collision other)
	{
		Destroy(body);
		Destroy(bodyCollider);
		Destroy(GetComponent<TimeKeptBody>());

		if (anim != null)
		{
			anim.SetTrigger(hitParam);
		}

		var target = other.collider.GetComponentInParent<Target>();
		if (target != null)
		{
			target.HitRespond();
		}
	}
}
