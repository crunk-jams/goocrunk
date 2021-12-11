using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
	public float TimeScale = 1;
	public bool HasFocus { get; private set; } = true;

	private void OnApplicationFocus(bool hasFocus)
	{
		this.HasFocus = hasFocus;
	}
}
