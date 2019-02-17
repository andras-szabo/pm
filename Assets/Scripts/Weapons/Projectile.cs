using UnityEngine;

public class Projectile : MonoWithCachedTransform, IProjectile
{
	[Range(1f, 100f)]
	public float speedPerSecond;

	public TrailRenderer trailRenderer;

	protected virtual void OnEnable()
	{
		if (trailRenderer != null)
		{
			trailRenderer.Clear();
		}
	}

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
