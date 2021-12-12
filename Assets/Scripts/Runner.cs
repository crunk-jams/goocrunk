using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Runner : MonoBehaviour
{
	[SerializeField] private Rigidbody body;
	[SerializeField] private float minSpeed = 10;
	[SerializeField] private float maxSpeed = 100;
	[SerializeField] private float strafeSpeed = 10000;
	[Range(0f, 1f)] private float speedIntensity = 0;
	public int grounded = 0;

	private Vector3 pathStart = Vector3.zero;
	private Vector3 pathDirection = Vector3.zero;
	private float pathWidth = 0;

	private CheckpointMessage savedCheckpoint = null;

	private void Start()
	{
		GotoCheckpointIfAllowed();
	}

	void Update()
	{
#if UNITY_EDITOR
		speedIntensity = DebugUpdateSpeedIntensity(speedIntensity);
#endif

		speedIntensity = Mathf.Clamp01(speedIntensity);
		var speed = Mathf.Lerp(minSpeed, maxSpeed, speedIntensity);

#if UNITY_EDITOR
		speed = DebugUpdateSpeed(speed);
#endif

		if (grounded >= 1)
		{
			body.velocity = (transform.forward * speed);
		}
		else
		{
			var noUpForward = transform.forward;
			noUpForward.y = 0;
			body.velocity = (noUpForward * speed) + new Vector3(0, body.velocity.y, 0);
		}

		var strafe = Input.GetAxis("Horizontal");
		if (Mathf.Abs(strafe) > 0.001 && grounded > 0)
		{
			var strafeForce = transform.right * strafe * strafeSpeed;
			body.velocity += strafeForce;
			KeepOnPath(strafeForce);
		}

		//TODO REMove
		KeepOnPath(Vector3.right);

	}

	public void SetPathStats(Vector3 start, Vector3 direction, float width)
	{
		pathStart = start;
		pathDirection = direction;
		pathWidth = width;
	}

	private void KeepOnPath(Vector3 strafe)
	{
		var pathRight = Vector3.Cross(transform.up, pathDirection).normalized;
		var onPathRight = Vector3.Project(transform.position - pathStart, pathRight);
		var pathHalfWidth = pathWidth / 2;
		if (onPathRight.sqrMagnitude > pathHalfWidth * pathHalfWidth && Vector3.Dot(strafe, onPathRight) > 0)
		{
			var sign = Vector3.Dot(onPathRight, pathRight) >= 0 ? 1 : -1;
			var newPos = transform.position;
			newPos -= onPathRight;
			newPos += pathRight * pathHalfWidth * sign;
			transform.position = newPos;
		}
	}

	public void ResetToCheckpoint()
	{
		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		FindObjectOfType<LevelManager>().BeginLevel();
	}

	public void GotoCheckpointIfAllowed()
	{
		var checkpointMessage = FindObjectOfType<CheckpointMessage>();
		if (checkpointMessage != null)
		{
			if (checkpointMessage.level == SceneManager.GetActiveScene().name)
			{
				savedCheckpoint = checkpointMessage;
				transform.position = checkpointMessage.position;
				transform.rotation = checkpointMessage.rotation;
				speedIntensity = checkpointMessage.speedIntensity;
				pathStart = checkpointMessage.pathStart = pathStart;
				pathDirection = checkpointMessage.pathDirection;
				pathWidth = checkpointMessage.pathWidth;
			}
			else
			{
				Destroy(checkpointMessage.gameObject);
			}
		}
	}

	public void SetCheckpoint(Checkpoint checkpoint)
	{
		var checkpointMessage = new GameObject().AddComponent<CheckpointMessage>();
		checkpointMessage.level = SceneManager.GetActiveScene().name;
		checkpointMessage.position = transform.position;
		checkpointMessage.rotation = transform.rotation;
		checkpointMessage.speedIntensity = speedIntensity;
		checkpointMessage.pathStart = pathStart;
		checkpointMessage.pathDirection = pathDirection;
		checkpointMessage.pathWidth = pathWidth;

		savedCheckpoint = checkpointMessage;
	}

	float DebugUpdateSpeedIntensity(float intensity)
	{
		// Increase speed
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			intensity += 0.1f;
		}

		// Decrease speed
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			intensity -= 0.1f;
		}

		return intensity;
	}

	float DebugUpdateSpeed(float speed)
	{
		// Stop while holding key
		if (Input.GetKey(KeyCode.Space))
		{
			speed = 0;
		}

		return speed;
	}

	private void OnCollisionEnter(Collision other)
	{
		switch (LayerMask.LayerToName(other.gameObject.layer))
		{
			case "TrackFloor":
				grounded++;
				break;
			case "Obstacle":
				ResetToCheckpoint();
				break;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		switch (LayerMask.LayerToName(other.gameObject.layer))
		{
			case "TrackFloor":
				grounded--;
				grounded = Mathf.Max(grounded, 0);
				break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var checkpoint = other.GetComponent<Checkpoint>();
		if (checkpoint != null)
		{
			SetCheckpoint(checkpoint);
		}
	}
}
