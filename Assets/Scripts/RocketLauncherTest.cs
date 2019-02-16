using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherTest : MonoWithCachedTransform
{
	public ProjectileLauncher launcher;

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			var target = GameController.TryGetManager<IHUDManager>().LockOnManager.LockedOnTransform;

			if (target != null)
			{
				launcher.Launch(CachedTransform.rotation, target);
			}
		}
	}
}
