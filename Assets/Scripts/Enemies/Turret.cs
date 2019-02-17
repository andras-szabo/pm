using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WeaponController))]
public class Turret : MonoBehaviour 
{
	//TODO -- yeah get the player
	public Rigidbody player;

	float maxRange = 100f;

	[Range(0.1f, 5f)] public float shotCooldownSeconds = 1f;

	[Tooltip("Set it to 1.0 for perfect prediction; 0 for basically only hitting the target by chance.")]
	[Range(0f, 1f)] public float aimPrecision;

	[Tooltip("The higher this value, the lousier shot this turret.")] 
	[Range(0.2f, 3f)] public float aimScatter;

	//TODO: into bullet
	public float bulletSpeedUnitsPerSecond = 100f;

	private Vector3 _aimPosition;
	private float _elapsedSinceLastShot;
	private int _targetLayerMask;

	private WeaponController _cachedWC;
	private WeaponController CachedWeaponController { get { return _cachedWC ?? (_cachedWC = GetComponent<WeaponController>()); } }

	private Rigidbody _cachedRigidBody;
	private Rigidbody CachedRigidbody
	{
		get
		{
			return _cachedRigidBody ?? (_cachedRigidBody = GetComponent<Rigidbody>());
		}
	}

	private void Update()
	{
		TryShoot();
	}

	private bool TryShoot()
	{
		_elapsedSinceLastShot += Time.deltaTime;

		if (_elapsedSinceLastShot >= shotCooldownSeconds && CanSee(player.position))
		{
			if (TryCalculateAimPosition(out _aimPosition))
			{
				var scatterX = Random.Range(-aimScatter, aimScatter);
				var scatterY = Random.Range(-aimScatter, aimScatter);

				_aimPosition += (1f - aimPrecision) * new Vector3(scatterX, scatterY);

				CachedRigidbody.transform.LookAt(_aimPosition, Vector3.up);
				CachedWeaponController.Shoot(0, null);

				_elapsedSinceLastShot = 0f;
				return true;
			}
		}

		return false;
	}

	private bool CanSee(Vector3 targetPosition)
	{
		RaycastHit hit;

		if (Physics.Raycast(CachedRigidbody.position, targetPosition - CachedRigidbody.position,
						out hit, maxRange))
		{
			return hit.rigidbody == player;
		}

		return false;
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(_aimPosition, 0.2f);
			Gizmos.DrawLine(CachedRigidbody.position, _aimPosition);
		}
	}

	private bool TryCalculateAimPosition(out Vector3 aimPosition)
	{
		var p = player.position - CachedRigidbody.position;
		var v = player.velocity - CachedRigidbody.velocity;
		var s = bulletSpeedUnitsPerSecond;

		float t;
		if (TryCalculateTimeOfImpact(p, v, s, out t))
		{
			aimPosition = player.position + player.velocity * t;
			return true;
		}

		aimPosition = Vector3.zero;
		return false;
	}

	private bool TryCalculateTimeOfImpact(Vector3 p, Vector3 v, float s, out float t)
	{
		t = 0f;

		// |p + vt| = st					=> when solving for t, we get a quadratic expression:
		// t^2(v^2 - s^2) + t(2pv) + p^2	=> 
		// t1,2 = (-2pv +/- sqrt( 2pv^2 - 4(v^2 - s^2)p^2) / 2(v^2 - s^2)

		var a = Vector3.Dot(v, v) - Mathf.Pow(s, 2);
		var b = 2 * Vector3.Dot(p, v);
		var c = Vector3.Dot(p, p);

		var d = Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c);

		if (a != 0f && d >= 0f)
		{
			var tOne = (-b + d) / (a * 2);
			var tTwo = (-b - d) / (a * 2);

			t = Mathf.Max(tOne, tTwo);

			return true;
		}

		return false;
	}
}
