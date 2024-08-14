using System;
using UnityEngine;

namespace DefaultNamespace
{
	public class UnlockCursor : MonoBehaviour
	{
		private void Update()
		{
			if (Cursor.lockState != CursorLockMode.None)
			{
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}
}