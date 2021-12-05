
using UnityEngine;

public class TeleTarget : Target
{
	[SerializeField] private DirectionTrigger directionTrigger = null;

	public override void HitRespond()
	{
		var teleportee = FindObjectOfType<Teleportee>();
		teleportee.TeleportTo(transform);

		if (directionTrigger != null)
		{
			directionTrigger.GiveDirection(teleportee.transform);
		}

		base.HitRespond();
	}
}
