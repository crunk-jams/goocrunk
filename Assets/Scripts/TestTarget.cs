using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : Target
{
	public override void HitRespond()
	{
		Debug.Log("Hit Target" + transform.position);
	}
}
