using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoWithCachedTransform
{
	public Projectile projectile;

	[Tooltip("Weapons having the same tag can fire using a single shoot or launch command. Think of them as e.g. 'primary' or 'secondary' weapons.")]
	[Range(0, 3)]
	public int weaponTag;

	[Range(0f, 5f)]
	public float cooldownSeconds;

	private float _elapsedSinceLastShot = 0f;

	private void Update()
	{
		if (_elapsedSinceLastShot <= cooldownSeconds)
		{
			_elapsedSinceLastShot += Time.deltaTime;
		}
	}

	public bool TryLaunch(Transform launchParam)
	{
		//TODO pool

		if (_elapsedSinceLastShot > cooldownSeconds)
		{
			var newProjectile = Instantiate<Projectile>(projectile,
					CachedTransform.position, CachedTransform.rotation, null);

			newProjectile.Setup(launchParam);
			_elapsedSinceLastShot = 0f;
			return true;
		}

		return false;
	}
}
