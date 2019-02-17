public struct LineOfSightInfo
{
	public bool isEnemyInSight;
	public bool isLockedOn;
}

public interface ILockOnManager
{
	event System.Action<UnityEngine.Transform> OnLockOnChanged;
	event System.Action<LineOfSightInfo> OnTargetInSightChanged;

	float SecondsRequiredForLockOn { get; }
	UnityEngine.Vector2 LockTolerance { get; }

	float MaxLockOnDistance { get; }
	void UpdateLockStatus(HUDMarker marker, bool isTargetInLockedArea, float distanceToCentreSquared, bool hasLineOfSight);
	void RemoveMarker(HUDMarker marker);
}