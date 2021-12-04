using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
	public virtual void HitRespond()
	{
		Destroy(gameObject);
	}
}
