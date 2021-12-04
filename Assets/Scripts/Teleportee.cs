using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportee : MonoBehaviour
{
	public void TeleportTo(Transform target)
	{
		transform.position = target.transform.position;
	}
}
