using UnityEngine;

public class DealDamageOnImpact : MonoBehaviour
{
	public int damage;
	private bool _hasDealtDamage;

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

		Destroy(gameObject);
	}
}
