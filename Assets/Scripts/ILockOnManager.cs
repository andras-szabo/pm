public interface ILockOnManager
{
	float SecondsRequiredForLockOn { get; }
	UnityEngine.Vector2 LockTolerance { get; }
	UnityEngine.Transform LockedOnTransform { get; }

	float MaxLockOnDistance { get; }
	void UpdateLockStatus(HUDMarker marker, bool isTargetInLockedArea, float distanceToCentreSquared);
	void RemoveMarker(HUDMarker marker);
}