using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestroyOnImpact : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(this.gameObject);
		
		// OK how about this.
		// on collision, I talk to the collision system:
		// I hit collision.transform, dealing them X damage, such and such arguments.
		// And then I destroy myself.
		// Meanwhile, collidable dudes register with the CollisionManager, with their transform.
		// So the collisionManager can disambiguate.
		// Let's do this.
	}
}
