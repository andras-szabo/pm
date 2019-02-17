public interface ICameraManager : IManager
{
	UnityEngine.Rect RegisterAndGetViewportPixelRect(CustomCameraContainer camContainer);
	void Unregister(CustomCameraContainer camContainer);
}