using UnityEngine;

public class DealDamageOnImpact : PoolUser
{
	public int damage;
	private bool _hasDealtDamage;

	private void OnEnable()
	{
		_hasDealtDamage = false;	
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (_hasDealtDamage)
		{
			return;
		}

		var collisionManager = GameController.TryGetManager<ICollisionManager>();
		if (collisionManager != null)
		{
			collisionManager.ReportHit(collider.gameObject, damage);
		}

		_hasDealtDamage = true;

		Despawn();
	}
}
