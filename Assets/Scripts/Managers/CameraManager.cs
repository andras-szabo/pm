using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraManager
{
	private void Awake()
	{
		GameController.TryRegister<ICameraManager>(this);	
	}
}
