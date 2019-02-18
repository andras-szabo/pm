using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LockOnManager))]
[RequireComponent(typeof(CanvasScaler))]
public class HUDManager : MonoWithCachedTransform, IHUDManager
{
	public HUDMarker markerPrefab;
	public Camera viewCamera;
	public Compass compass;

	public HUDCustomCameraScreen[] customCameraScreens;

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
		compass.AddCustomHUDMarker(newMarker);
	}

	public void RemoveMarker(HUDMarker marker)
	{
		Destroy(marker.gameObject);
	}

	private void RecalculateScreenDimensions()
	{
		// This is assuming "scale with screen size" UI scale mode.
		// Details: https://docs.unity3d.com/ScriptReference/UI.CanvasScaler-matchWidthOrHeight.html

		var canvasScaler = GetComponent<CanvasScaler>();

		var logWidth = Mathf.Log10((float)Screen.width / canvasScaler.referenceResolution.x);
		var logHeight = Mathf.Log10((float)Screen.height / canvasScaler.referenceResolution.y);
		var logWeightedAvg = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
		var resolutionFactor = Mathf.Pow(10f, logWeightedAvg);

		var halfWidth = Screen.width / 2f / resolutionFactor;
		var halfHeight = Screen.height / 2f / resolutionFactor;

		_screenHalfDimensions = new Vector2(halfWidth, halfHeight);
	}

	public void ToggleCustomCameraScreen(CustomCameraType type, bool state)
	{
		var screen = GetScreenFor(type);
		if (screen != null) { screen.Toggle(state); }
	}

	private HUDCustomCameraScreen GetScreenFor(CustomCameraType type)
	{
		foreach (var camScreen in customCameraScreens)
		{
			if (camScreen.type == type)
			{
				return camScreen;
			}
		}

		return null;
	}

	public Rect GetPixelRectForCamViewport(CustomCameraType type)
	{
		var camScreen = GetScreenFor(type);
		if (camScreen != null) { return CalculateCamRect(camScreen.GetWorldCorners()); }
		return new Rect(0, 0, 0, 0);
	}

	private Rect CalculateCamRect(Vector3[] worldCorners)
	{
		return new Rect(worldCorners[0], worldCorners[2] - worldCorners[0]);
	}
}
