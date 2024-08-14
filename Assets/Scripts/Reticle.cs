using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
	public enum AimState
	{
		FreeAim,
		NockLocked,
		Recovering
	}

	private AimState state = AimState.FreeAim;
	public AimState State => state;

	private Vector3 offset = Vector3.zero;

	[SerializeField] private PlayerCamera cam = null;
	[SerializeField] private float camSensitivity = 180f;
	[SerializeField] private float horizontalTurnMax = 45;
	[SerializeField] private float verticalTurnMax = 45;

	[SerializeField] private GameObject apexIndicator;
	[SerializeField] private GameObject shortArcIndicator;
	[SerializeField] private GameObject longArcIndicator;
	[HideInInspector] public float pullStrength = 0;
	[HideInInspector] public bool shotReady = false;


	private TimeKeeper timeKeeper = null;

	void Start()
	{
		Cursor.visible = false;
		offset = transform.localPosition;
		timeKeeper = FindObjectOfType<TimeKeeper>();
	}


	private void Update()
	{
		if (!timeKeeper.HasFocus)
		{
			return;
		}

		switch (state)
		{
			case AimState.FreeAim:
				Cursor.lockState = CursorLockMode.Locked;
				apexIndicator.SetActive(false);
				break;
			case AimState.NockLocked:
				//Cursor.lockState = CursorLockMode.Confined;
				Cursor.lockState = CursorLockMode.Locked;
				apexIndicator.SetActive(shotReady);
				// Arduino changes
				//pullStrength = Mathf.Sqrt(Mathf.Clamp01(pullStrength));
				pullStrength = Mathf.Clamp01(pullStrength);
				apexIndicator.transform.position =
					(shortArcIndicator.transform.position * (1- pullStrength)) +
					(longArcIndicator.transform.position * pullStrength);
				break;
		}

		var scaledSensitivity = camSensitivity / Time.timeScale;

		// Allow vertical rotation while aiming freely. After shooting the player needs to return mouse to it's old y.
		// Arduino changes
		//if (state == AimState.FreeAim)
		if (true)
		{
			float verticalRotation = Input.GetAxis("Mouse Y") * Time.deltaTime * scaledSensitivity;
			verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
			cam.transform.Rotate(-cam.Player.right * verticalRotation, Space.World);
		}

		// Allow horizontal rotation, and all rotation clamping at all times.
		float horizontalRotation = Input.GetAxis("Mouse X") * Time.deltaTime * scaledSensitivity;
		horizontalRotation = Mathf.Clamp(horizontalRotation, -90, 90);
		cam.transform.Rotate(cam.Player.up * horizontalRotation, Space.World);

		cam.transform.LookAt(cam.transform.position + cam.transform.forward, cam.Player.up);

		var noUp = (cam.transform.forward - Vector3.Project(cam.transform.forward, cam.Player.up)).normalized;
		var noRight = (cam.transform.forward - Vector3.Project(cam.transform.forward, cam.Player.right)).normalized;

		var dotRight = Vector3.Dot(noUp, cam.Player.right);
		var dotUp = Vector3.Dot(noRight, cam.Player.up);

		var horizontalTurnTotal = Mathf.Abs(90 - (Mathf.Acos(dotRight) * Mathf.Rad2Deg));
		if (horizontalTurnTotal > horizontalTurnMax)
		{
			var sign = dotRight >= 0 ? 1 : -1;
			cam.transform.Rotate(cam.Player.up * (horizontalTurnMax - horizontalTurnTotal) * sign, Space.World);
		}

		var verticalTurnTotal = Mathf.Abs(90 - (Mathf.Acos(dotUp) * Mathf.Rad2Deg));
		if (verticalTurnTotal > verticalTurnMax)
		{
			var sign = dotUp >= 0 ? 1 : -1;
			cam.transform.Rotate(-cam.Player.right * (verticalTurnMax - verticalTurnTotal) * sign, Space.World);
		}
	}

	public void Unlock()
	{
		state = AimState.FreeAim;
	}

	public void Lock()
	{
		state = AimState.NockLocked;
	}

	public void StartRecover()
	{
		state = AimState.Recovering;
	}

	public void EndRecover()
	{
		if (state == AimState.Recovering)
		{
			state = AimState.FreeAim;
		}
	}
}
