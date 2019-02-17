using UnityEngine;

public class Projectile : MonoWithCachedTransform, IProjectile
{
	[Range(1f, 100f)]
	public float speedPerSecond;

	public virtual void Setup(Transform transform)
	{
	}

	protected virtual void Update()
	{
		CachedTransform.position += CachedTransform.forward * speedPerSecond * Time.deltaTime;
	}
}

public interface IProjectile
{
	void Setup(Transform transform);
}
