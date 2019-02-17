using UnityEngine;

public class TransformChaser : Projectile
{
	private Transform _target;

	public override void Setup(Transform target)
	{
		_target = target;
	}

	[Range(30f, 180f)]
	public float maxRotationAnglesPerSec;

	protected override void Update()
	{
		if (_target != null)
		{
			var toTarget = (_target.position - CachedTransform.position).normalized;
			var angleDifference = Vector3.Angle(CachedTransform.forward, toTarget);

			if (angleDifference > 0.25f)
			{
				RotateTowards(toTarget, angleDifference);
			}
		}

		base.Update();
	}

	private void RotateTowards(Vector3 toTarget, float angleDifference)
	{
		var maxRotationPerFrame = Time.deltaTime * maxRotationAnglesPerSec;
		var slerpFactor = angleDifference > maxRotationPerFrame ? maxRotationPerFrame / angleDifference :
																  angleDifference / maxRotationPerFrame;

		CachedTransform.rotation = Quaternion.Slerp(CachedTransform.rotation,
													Quaternion.LookRotation(toTarget, Vector3.up),
													slerpFactor);
	}
}
