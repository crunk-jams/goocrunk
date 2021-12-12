using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
	[SerializeField] private UnityEvent onHit = null;
	[SerializeField] private Animator anim = null;

	public virtual void HitRespond()
	{
		onHit.Invoke();

		if (anim != null)
		{
			anim.SetTrigger("Broken");
			AudioManager.Instance.HitTarget();

			var colliders = GetComponentsInChildren<Collider>();
			if (colliders != null && colliders.Length > 0)
			{
				foreach (var col in colliders)
				{
					col.enabled = false;
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
