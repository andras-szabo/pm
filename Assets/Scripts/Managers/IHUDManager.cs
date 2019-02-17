using UnityEngine;

public interface IHUDManager : IManager 
{ 
	ILockOnManager LockOnManager { get; }
	void SpawnMarker(UnityEngine.Transform target, Color color, string label, bool isLockable);
	void RemoveMarker(HUDMarker marker);
	Rect GetPixelRectForCamViewport(CustomCameraType type);
	void ToggleCustomCameraScreen(CustomCameraType type, bool state);
}
