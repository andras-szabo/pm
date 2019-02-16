using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoWithCachedTransform, IProjectile
{
	public virtual void Setup(Transform transform)
	{
	}
}

public interface IProjectile
{
	void Setup(Transform transform);
}
