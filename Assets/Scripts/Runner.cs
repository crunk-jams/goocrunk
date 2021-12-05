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
	private int grounded = 0;

	private Vector3 pathStart = Vector3.zero;
	private Vector3 pathDirection = Vector3.zero;
	private float pathWidth = 0;

	public Checkpoint checkpoint = null;

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

		body.velocity = (transform.forward * speed) + new Vector3(0, body.velocity.y, 0);

		var strafe = Input.GetAxis("Horizontal");
		if (Mathf.Abs(strafeSpeed) > 0.001)
		{
			body.velocity += transform.right * strafe * strafeSpeed;
		}

		KeepOnPath();
	}

	public void SetPathStats(Vector3 start, Vector3 direction, float width)
	{
		pathStart = start;
		pathDirection = direction;
		pathWidth = width;
	}

	private void KeepOnPath()
	{
		var pathRight = Vector3.Cross(transform.up, pathDirection).normalized;
		var onPathRight = Vector3.Project(transform.position - pathStart, pathRight);
		var pathHalfWidth = pathWidth / 2;
		if (onPathRight.sqrMagnitude > pathHalfWidth * pathHalfWidth)
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
		/*body.velocity = Vector3.zero;
		var resetPosition = Vector3.zero;
		if (checkpoint != null)
		{
			resetPosition = checkpoint.transform.position;
		}
		transform.position = resetPosition;*/
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
				break;
		}
	}
}
