using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	private Rigidbody body = null;
	private Collider bodyCollider = null;
	private void Start()
	{
		body = GetComponent<Rigidbody>();
		bodyCollider = GetComponent<Collider>();
		bodyCollider.enabled = false;
	}

	private void Update()
	{
		var loosed = body != null && body.useGravity && !body.isKinematic;
		if (loosed)
		{
			transform.forward = body.velocity.normalized;
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

		var target = other.collider.GetComponentInParent<Target>();
		if (target != null)
		{
			target.HitRespond();
		}
	}
}
