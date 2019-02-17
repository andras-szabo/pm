using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestroyOnImpact : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(this.gameObject);
	}
}
