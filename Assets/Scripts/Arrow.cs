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

		bodyCollider.enabled = loosed;
	}

	private void OnCollisionEnter(Collision other)
	{
		Destroy(body);
	}
}
