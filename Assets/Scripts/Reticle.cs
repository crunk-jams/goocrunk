using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
	private float trackChangedTime = 0f;
	private Vector3 oldPlayerEuler = Vector3.zero;

	[SerializeField] private CursorLockMode cursorMode = CursorLockMode.Confined;
	[SerializeField] private PlayerCamera cam = null;
	[SerializeField] private float camSensitivity = 180f;
	[SerializeField] private float horizontalTurnMax = 45;
	[SerializeField] private float verticalTurnMax = 45;
	[SerializeField] private float trackTurnDuration = 0.3f;

	[SerializeField] private GameObject apexIndicator;
	[SerializeField] private GameObject shortArcIndicator;
	[SerializeField] private GameObject longArcIndicator;
	[SerializeField] private bool canDebug;
	[SerializeField] private TextMeshProUGUI debugViewLimits;
	[HideInInspector] public float pullStrength = 0;
	[HideInInspector] public bool shotReady = false;

	[Header("Gun Smoothing")]
	[SerializeField] private bool simpleSmoothing = false;
	[SerializeField] private int smoothFrames = 0;

	[Header("Gun Simulation Testing")]
	[SerializeField] private float simRestNoise = 0f;
	[SerializeField] private float simRecoil = 0f;

	private TimeKeeper timeKeeper = null;
	private bool simulateGun = false;
	private bool applySmoothing = true;
	private Vector2 accumulatedRotation = Vector2.zero;
	private List<Vector2> cachedFrames = new List<Vector2>();

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

		if (cachedFrames.Count < smoothFrames)
		{
			for (int i = cachedFrames.Count; i < smoothFrames; i++) {
				cachedFrames.Insert(0, Vector2.zero);
			}
		}
		else if (cachedFrames.Count > smoothFrames)
		{
			while (cachedFrames.Count > smoothFrames) {
				cachedFrames.RemoveAt(0);
			}
		}

		if (canDebug)
		{
			bool showDebug = Input.GetKey(KeyCode.LeftShift);
			if (debugViewLimits.gameObject.activeSelf != showDebug)
			{
				debugViewLimits.gameObject.SetActive(showDebug);
			}

			if (showDebug)
			{
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					horizontalTurnMax--;
				}

				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					horizontalTurnMax++;
				}

				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					verticalTurnMax--;
				}

				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					verticalTurnMax++;
				}

				debugViewLimits.text = $"{horizontalTurnMax}x{verticalTurnMax}";
			}
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			simulateGun = !simulateGun;
		}

		if (Input.GetKeyDown(KeyCode.H))
		{
			applySmoothing = !applySmoothing;
		}

		switch (state)
		{
			case AimState.FreeAim:
				Cursor.lockState = cursorMode;
				//Cursor.lockState = CursorLockMode.Locked;
				apexIndicator.SetActive(false);
				break;
			case AimState.NockLocked:
				Cursor.lockState = cursorMode;
				//Cursor.lockState = CursorLockMode.Locked;
				apexIndicator.SetActive(shotReady);
				// Arduino changes
				//pullStrength = Mathf.Sqrt(Mathf.Clamp01(pullStrength));
				pullStrength = Mathf.Clamp01(pullStrength);
				apexIndicator.transform.position =
					(shortArcIndicator.transform.position * (1- pullStrength)) +
					(longArcIndicator.transform.position * pullStrength);
				break;
		}

		Vector2 newRotation = cursorMode == CursorLockMode.Locked
			? LookMouseDelta()
			: LookMousePos();

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

		if (cachedFrames.Count > 0)
		{
			for (int i = 0; i < cachedFrames.Count - 1; i++)
			{
				cachedFrames[i] = cachedFrames[i + 1];
			}
			cachedFrames[cachedFrames.Count - 1] = newRotation;
		}
	}

	private Vector2 LookMouseDelta()
	{
		var scaledSensitivity = camSensitivity / Time.timeScale;
		var newRotation = Vector2.zero;

		// Allow vertical rotation while aiming freely. After shooting the player needs to return mouse to it's old y.
		// Arduino changes
		//if (state == AimState.FreeAim)
		if (true)
		{
			float verticalRotation = Input.GetAxis("Mouse Y") * Time.deltaTime * scaledSensitivity;
			verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
			if (simulateGun) { verticalRotation += UnityEngine.Random.Range(-simRestNoise, simRestNoise) * Time.deltaTime; }
			newRotation.y = verticalRotation;
			verticalRotation = DenoiseRest(verticalRotation, true);
			cam.transform.Rotate(-cam.Player.right * verticalRotation, Space.World);
		}

		// Allow horizontal rotation, and all rotation clamping at all times.
		float horizontalRotation = Input.GetAxis("Mouse X") * Time.deltaTime * scaledSensitivity;
		horizontalRotation = Mathf.Clamp(horizontalRotation, -90, 90);
		newRotation.x = horizontalRotation;
		if (simulateGun) { horizontalRotation += UnityEngine.Random.Range(-simRestNoise, simRestNoise) * Time.deltaTime; }
		horizontalRotation = DenoiseRest(horizontalRotation, false);
		cam.transform.Rotate(cam.Player.up * horizontalRotation, Space.World);

		cam.transform.LookAt(cam.transform.position + cam.transform.forward, cam.Player.up);
		return newRotation;
	}

	private Vector2 LookMousePos()
	{
		var maxRot = new Vector2(horizontalTurnMax, verticalTurnMax);
		var normMousePos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		var newRotation = new Vector2(
				((1f - normMousePos.x) * -maxRot.x) + (normMousePos.x *  maxRot.x),
				((1f - normMousePos.y) *  maxRot.y) + (normMousePos.y * -maxRot.y));

		var verticalRotation = newRotation.y;
		if (simulateGun) { verticalRotation += UnityEngine.Random.Range(-simRestNoise, simRestNoise) * Time.deltaTime; }
		newRotation.y = verticalRotation;
		verticalRotation = DenoiseRest(verticalRotation, true);

		var horizontalRotation = newRotation.x;
		if (simulateGun) { horizontalRotation += UnityEngine.Random.Range(-simRestNoise, simRestNoise) * Time.deltaTime; }
		newRotation.x = horizontalRotation;
		horizontalRotation = DenoiseRest(horizontalRotation, false);

		var eulerWeight = trackTurnDuration > 0
			? Mathf.Clamp01((Time.time - trackChangedTime) / trackTurnDuration)
			: 0; // If zero or less do no turn for track
		var oldPlayerEulerY = oldPlayerEuler.y < 180 ? oldPlayerEuler.y : oldPlayerEuler.y - 360;
		var newPlayerEulerY = cam.Player.transform.eulerAngles.y < 180 ? cam.Player.transform.eulerAngles.y : cam.Player.transform.eulerAngles.y - 360;
		var playerEulerY = ((1f - eulerWeight) * oldPlayerEulerY) + (eulerWeight * newPlayerEulerY);
		cam.transform.eulerAngles = new Vector3(verticalRotation, horizontalRotation, 0) + new Vector3(0, playerEulerY, 0);

		cam.transform.LookAt(cam.transform.position + cam.transform.forward, cam.Player.up);
		return newRotation;
	}


	public float DenoiseRest(float attemptRotation, bool vertical)
	{
		if (!applySmoothing)
		{
			return attemptRotation;
		}

		var sum = vertical
			? new Vector2(0, attemptRotation)
			: new Vector2(attemptRotation, 0);


		if (simpleSmoothing)
		{
			for (int i = 0; i < cachedFrames.Count; i++)
			{
				sum += cachedFrames[i];
			}
		}
		else
		{
			var factor = (float)cachedFrames.Count;
			for (int i = 0; i < cachedFrames.Count; i++)
			{
				sum += cachedFrames[i] * (i / factor);
			}
		}

		var avg = sum / (cachedFrames.Count + 1);
		return vertical ? avg.y : avg.x;
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

	public void TrackChanged()
	{
		trackChangedTime = Time.time;
		oldPlayerEuler = cam.Player.transform.eulerAngles;
	}
}
