using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DieInAPit : MonoBehaviour
{
	private PitOfDeath pit = null;
	[SerializeField] private bool destroyOnDie = true;
	[SerializeField] private UnityEvent onDie = null;

	// TODO @Sam Falling off the ledge before the first portal in Level 5 gets stuck in loop of death.
	private void Start()
	{
		pit = FindObjectOfType<PitOfDeath>();
	}

	private void Update()
	{
		if (transform.position.y < pit.transform.position.y)
		{
			onDie.Invoke();
			if (destroyOnDie)
			{
				Destroy(gameObject);
			}
		}
	}
}
