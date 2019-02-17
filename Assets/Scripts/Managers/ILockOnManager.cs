public interface ILockOnManager
{
	event System.Action<UnityEngine.Transform> OnLockOnChanged;

	float SecondsRequiredForLockOn { get; }
	UnityEngine.Vector2 LockTolerance { get; }

	float MaxLockOnDistance { get; }
	void UpdateLockStatus(HUDMarker marker, bool isTargetInLockedArea, float distanceToCentreSquared);
	void RemoveMarker(HUDMarker marker);
}