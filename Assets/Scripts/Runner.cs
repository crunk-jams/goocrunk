using UnityEngine;
using UnityEngine.SceneManagement;

public class Runner : MonoBehaviour
{
	[SerializeField] private Rigidbody body;
	[SerializeField] private float minSpeed = 10;
	[SerializeField] private float maxSpeed = 100;
	[SerializeField] private float strafeSpeed = 10000;
	[Range(0f, 1f)] private float speedIntensity = 0;

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
			body.AddForce(transform.right * strafe * strafeSpeed * Time.deltaTime);
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
}
