using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoWithCachedTransform
{
	public Projectile projectile;
	public bool usePool = true;

	[Tooltip("Weapons having the same tag can fire using a single shoot or launch command. Think of them as e.g. 'primary' or 'secondary' weapons.")]
	[Range(0, 3)]
	public int weaponTag;

	[Range(0f, 5f)]
	public float cooldownSeconds;

	private IPoolManager _pool;

	public float CooldownRate
	{
		get
		{
			return Mathf.Clamp01(_elapsedSinceLastShot / cooldownSeconds);
		}
	}

	private float _elapsedSinceLastShot = 5f;

	private void Start()
	{
		_pool = GameController.TryGetManager<IPoolManager>();
	}

	private void Update()
	{
		if (_elapsedSinceLastShot <= cooldownSeconds)
		{
			_elapsedSinceLastShot += Time.deltaTime;
		}
	}

	public bool TryLaunch(Transform launchParam)
	{
		if (_elapsedSinceLastShot > cooldownSeconds)
		{
			if (usePool && _pool != null)
			{
				var projectileFromPool = _pool.Spawn<Projectile>(projectile, CachedTransform.position, CachedTransform.rotation, null);

				if (projectileFromPool == null)
				{
					Debug.Break();
				}

				projectileFromPool.Setup(launchParam);
			}
			else
			{
				var newProjectile = Instantiate<Projectile>(projectile, CachedTransform.position, CachedTransform.rotation, null);
				newProjectile.Setup(launchParam);
			}

			_elapsedSinceLastShot = 0f;
			return true;
		}

		return false;
	}
}
