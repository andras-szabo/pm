using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : ICollisionManager
{
	private Dictionary<int, ICollidable> _collidablesByID = new Dictionary<int, ICollidable>();

	public void Register(GameObject gameObject, ICollidable collidable)
	{
		if (gameObject != null)
		{
			_collidablesByID.Add(gameObject.GetInstanceID(), collidable);
		}
	}

	public void Unregister(GameObject gameObject)
	{
		if (gameObject != null)
		{
			_collidablesByID.Remove(gameObject.GetInstanceID());
		}
	}

	public void ReportHit(GameObject gameObject, int damage)
	{
		ICollidable collidable;
		if (_collidablesByID.TryGetValue(gameObject.GetInstanceID(), out collidable))
		{
			collidable.Hit(damage);
		}
	}
}

public interface ICollidable
{
	void Hit(int damage);
}