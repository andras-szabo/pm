using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraContainer : MonoBehaviour 
{
	public Camera customCamera;
	public CustomCameraType type;

	public void Show() { customCamera.enabled = true; }
	public void Hide() { customCamera.enabled = false; }

	private void OnEnable()
	{
		Hide();
	}

	private void Start()
	{
		RegisterAndSetupViewport();
	}

	private void OnDestroy()
	{
		Unregister();
	}

	private void RegisterAndSetupViewport()
	{
		var camManager = GameController.TryGetManager<ICameraManager>();
		if (camManager != null)
		{
			customCamera.pixelRect = camManager.RegisterAndGetViewportPixelRect(this);
		}
	}

	private void Unregister()
	{
		var camManager = GameController.TryGetManager<ICameraManager>();
		if (camManager != null)
		{
			camManager.Unregister(this);
		}
	}
}

public enum CustomCameraType
{
	None,
	MissileCam
}
