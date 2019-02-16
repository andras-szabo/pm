using UnityEngine;

public class TransformChaser : MonoWithCachedTransform
{
	private Transform _target;

	public void Setup(Transform target)
	{
		_target = target;
	}

	[Range(30f, 180f)]
	public float maxRotationAnglesPerSec;

	[Range(1f, 100f)]
	public float speedPerSecond;

	private void Update()
	{
		if (_target != null)
		{
			var toTarget = (_target.position - CachedTransform.position).normalized;
			var angleDifference = Vector3.Angle(CachedTransform.forward, toTarget);

			if (angleDifference > 1f)
			{
				RotateTowards(toTarget, angleDifference);
			}

			CachedTransform.position += CachedTransform.forward * speedPerSecond * Time.deltaTime;
		}
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
