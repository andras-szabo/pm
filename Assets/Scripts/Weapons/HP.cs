using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour, ICollidable
{
	public int hitPoints = 100;
	public bool destroyWhenHPzero;

	private int _startingHP;

	public event System.Action<HPInfo> OnHitPointsChanged;

	public HPInfo GetHPInfo()
	{
		return new HPInfo { current = hitPoints, max = _startingHP };
	}

	private void Awake()
	{
		_startingHP = hitPoints;
	}

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
			hitPoints = System.Math.Max(0, hitPoints - damage);
			OnHitPointsChanged?.Invoke(new HPInfo { current = hitPoints, max = _startingHP });
			if (hitPoints <= 0 && destroyWhenHPzero)
			{
				Destroy(gameObject);
			}
		}
	}
}
