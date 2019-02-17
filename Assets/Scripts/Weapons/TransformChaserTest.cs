using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformChaserTest : MonoBehaviour
{
	public Transform target;
	
	private void Start()
	{
		var chaser = GetComponent<TransformChaser>();
		chaser.Setup(target);
	}
}
