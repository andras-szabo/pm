using UnityEngine;

public class DealDamageOnImpact : MonoBehaviour
{
	public int damage;
	private bool _hasDealtDamage;

	private void OnCollisionEnter(Collision collision)
	{
		if (_hasDealtDamage)
		{
			print("There are probably multiple colliders on this GameObject.");
			return;
		}

		var collisionManager = GameController.TryGetManager<ICollisionManager>();
		if (collisionManager != null)
		{
			collisionManager.ReportHit(collision.gameObject, damage);
		}

		_hasDealtDamage = true;

		Destroy(gameObject);
	}
}
