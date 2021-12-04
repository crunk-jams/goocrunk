
public class TeleTarget : Target
{
	public override void HitRespond()
	{
		var teleportee = FindObjectOfType<Teleportee>();
		teleportee.TeleportTo(transform);

		base.HitRespond();
	}
}
