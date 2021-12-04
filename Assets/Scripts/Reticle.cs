using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
	public enum AimState
	{
		FreeMove,
		NockLocked
	}

	private AimState state = AimState.FreeMove;
	public AimState State => state;

	[SerializeField] private Camera camera = null;
	private Vector3 offset = Vector3.zero;

	void Start()
	{
		Cursor.visible = false;
		offset = transform.localPosition;
	}

	private void Update()
	{
		switch (state)
		{
			case AimState.FreeMove:
				var mousePos = Input.mousePosition;
				mousePos.z = offset.z;
				transform.position = camera.ScreenToWorldPoint(mousePos);
				break;
			case AimState.NockLocked:
				break;
		}
	}

	public void Lock()
	{
		state = AimState.NockLocked;
	}

	public void Unlock()
	{
		state = AimState.FreeMove;
	}
}
