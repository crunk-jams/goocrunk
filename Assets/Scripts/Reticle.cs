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

	private Vector3 offset = Vector3.zero;

	[SerializeField] private PlayerCamera cam = null;
	[SerializeField] private float camSensitivity = 180f;
	[SerializeField] private float horizontalTurnMax = 45;
	[SerializeField] private float verticalTurnMax = 45;

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
				Cursor.lockState = CursorLockMode.Locked;
				AimCamera();
				break;
			case AimState.NockLocked:
				Cursor.lockState = CursorLockMode.Confined;
				break;
		}
	}

	private void AimCamera()
	{
		float verticalRotation = Input.GetAxis("Mouse Y") * Time.deltaTime * camSensitivity;
		float horizontalRotation = Input.GetAxis("Mouse X") * Time.deltaTime * camSensitivity;

		verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
		horizontalRotation = Mathf.Clamp(horizontalRotation, -90, 90);

		cam.transform.Rotate(cam.Player.up * horizontalRotation);
		cam.transform.Rotate(-cam.Player.right * verticalRotation);
		cam.transform.LookAt(cam.transform.position + cam.transform.forward, cam.Player.up);

		var noUp = (cam.transform.forward - Vector3.Project(cam.transform.forward, cam.Player.up)).normalized;
		var noRight = (cam.transform.forward - Vector3.Project(cam.transform.forward, cam.Player.right)).normalized;

		var dotRight = Vector3.Dot(noUp, cam.Player.right);
		var dotUp = Vector3.Dot(noRight, cam.Player.up);

		var horizontalTurnTotal = Mathf.Abs(90 - (Mathf.Acos(dotRight) * Mathf.Rad2Deg));
		if (horizontalTurnTotal > horizontalTurnMax)
		{
			var sign = dotRight >= 0 ? 1 : -1;
			cam.transform.Rotate(cam.Player.up * (horizontalTurnMax - horizontalTurnTotal) * sign);
		}

		var verticalTurnTotal = Mathf.Abs(90 - (Mathf.Acos(dotUp) * Mathf.Rad2Deg));
		if (verticalTurnTotal > verticalTurnMax)
		{
			var sign = dotUp >= 0 ? 1 : -1;
			cam.transform.Rotate(-cam.Player.right * (verticalTurnMax - verticalTurnTotal) * sign);
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
