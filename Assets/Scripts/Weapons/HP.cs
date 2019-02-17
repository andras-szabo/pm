using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour, ICollidable
{
	public int hitPoints = 100;
	public bool destroyWhenHPzero;

	public event System.Action<int> OnHitPointsChanged;

	private void Start()
	{
		var collisionManager = GameController.TryGetManager<ICollisionManager>();
		if (collisionManager != null)
		{
			collisionManager.Register(gameObject, this);
		}
	}

	private void OnDestroy()
	{
		var collisionManager = GameController.TryGetManager<ICollisionManager>();
		if (collisionManager != null)
		{
			collisionManager.Unregister(gameObject);
		}
	}

	public void Hit(int damage)
	{
		if (hitPoints >= 0 && damage > 0)
		{
			hitPoints -= damage;
			OnHitPointsChanged?.Invoke(hitPoints);
			if (hitPoints <= 0 && destroyWhenHPzero)
			{
				Destroy(gameObject);
			}
		}
	}
}
