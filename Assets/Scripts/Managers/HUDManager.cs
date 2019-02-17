using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LockOnManager))]
[RequireComponent(typeof(CanvasScaler))]
public class HUDManager : MonoWithCachedTransform, IHUDManager
{
	// TODO Pool
	public HUDMarker markerPrefab;
	public Camera viewCamera;

	private Vector2 _screenHalfDimensions;

	private LockOnManager _lockOnManager;
	public ILockOnManager LockOnManager
	{
		get { return _lockOnManager ?? (_lockOnManager = GetComponent<LockOnManager>()); }
	}

	private void Awake()
	{
		RecalculateScreenDimensions();
		GameController.TryRegister<IHUDManager>(this);
	}

	public void SpawnMarker(Transform target, Color color, string label, bool isLockable)
	{
		var newMarker = Instantiate<HUDMarker>(markerPrefab, CachedTransform);
		newMarker.Setup(viewCamera, target, _screenHalfDimensions, color, label, isLockable, this);
	}

	//TODO pool
	public void RemoveMarker(HUDMarker marker)
	{
		Destroy(marker.gameObject);
	}

	private void RecalculateScreenDimensions()
	{
		var canvasScaler = GetComponent<CanvasScaler>();

		var logWidth = Mathf.Log10((float)Screen.width / canvasScaler.referenceResolution.x);
		var logHeight = Mathf.Log10((float)Screen.height / canvasScaler.referenceResolution.y);
		var logWeightedAvg = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
		var resolutionFactor = Mathf.Pow(10f, logWeightedAvg);

		var halfWidth = Screen.width / 2f / resolutionFactor;
		var halfHeight = Screen.height / 2f / resolutionFactor;

		_screenHalfDimensions = new Vector2(halfWidth, halfHeight);
	}
}
