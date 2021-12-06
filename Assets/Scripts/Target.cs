using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
	[SerializeField] private UnityEvent onHit = null;

	public virtual void HitRespond()
	{
		onHit.Invoke();
		Destroy(gameObject);
	}
}
