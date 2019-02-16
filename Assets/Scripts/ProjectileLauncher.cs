using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoWithCachedTransform
{
	public Projectile projectile;

	public void Launch(Quaternion forward, Transform launchParam)
	{
		//TODO pool
		var newProjectile = Instantiate<Projectile>(projectile,
				CachedTransform.position, forward, null);

		newProjectile.Setup(launchParam);
	}
}
