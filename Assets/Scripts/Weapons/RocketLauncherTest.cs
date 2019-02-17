using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherTest : MonoWithCachedTransform
{
	public WeaponController weapons;

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			var target = GameController.TryGetManager<IHUDManager>().LockOnManager.LockedOnTransform;

			if (target != null)
			{
				weapons.Shoot(1, target);
			}
		}

		if (Input.GetMouseButton(0))
		{
			weapons.Shoot(0, null);
		}
	}
}
