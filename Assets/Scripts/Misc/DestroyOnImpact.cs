using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestroyOnImpact : PoolUser
{
	private void OnCollisionEnter(Collision collision)
	{
		Despawn();
	}
}
