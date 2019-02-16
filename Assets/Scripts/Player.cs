using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoWithCachedTransform
{
	private void Start()
	{
		GameController.RegisterPlayerTransform(CachedTransform);
	}
}
