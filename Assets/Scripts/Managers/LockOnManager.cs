using System;
using System.Collections.Generic;
using UnityEngine;

public class LockOnManager : MonoBehaviour, ILockOnManager
{
	private HUDMarker _lockedOnHudMarker;
	public HUDMarker LockedOnHUDMarker
	{
		get { return _lockedOnHudMarker; }
		set
		{
			_lockedOnHudMarker = value;
			OnLockOnChanged?.Invoke(_lockedOnHudMarker != null ? _lockedOnHudMarker.Target : null);
		}
	}
	public bool IsLockedOn { get { return _lockedOnHudMarker != null; } }

	public event Action<Transform> OnLockOnChanged;
	public event Action<LineOfSightInfo> OnTargetInSightChanged;

	[Range(0.1f, 1f)]
	public float lockOnToleranceH;

	[Range(0.1f, 1f)]
	public float lockOnToleranceV;

	[Range(0.1f, 4f)]
	public float secondsRequiredForLockOn;

	[Range(10f, 100f)]
	public float maxLockOnDistance;

	public float SecondsRequiredForLockOn { get { return secondsRequiredForLockOn; } }
	public Vector2 LockTolerance { get { return new Vector2(lockOnToleranceH, lockOnToleranceV); } }
	public float MaxLockOnDistance { get { return maxLockOnDistance; } }

	private HUDMarker _markerClosestToScreenCentre;
	private float _shortestDistanceToScreenCentreSquared = 1f;
	private bool _isTargetInLockArea;
	private List<HUDMarker> _inactiveMarkers = new List<HUDMarker>();
	private bool _dropLockOn;

	private void Update()
	{
		if (IsLockedOn && Input.GetKeyDown(KeyCode.LeftShift))
		{
			_dropLockOn = true;
		}
	}

	public void RemoveMarker(HUDMarker marker)
	{
		if (_lockedOnHudMarker == marker)
		{
			LockedOnHUDMarker = null;
		}
	}

	bool _hasLineOfSight;

	public void UpdateLockStatus(HUDMarker marker, bool isTargetInLockedArea, float distanceToCentreSquared, bool hasLineOfSight)
	{
		if (distanceToCentreSquared < _shortestDistanceToScreenCentreSquared)
		{
			if (_markerClosestToScreenCentre != null)
			{
				_inactiveMarkers.Add(_markerClosestToScreenCentre);
			}

			_markerClosestToScreenCentre = marker;
			_shortestDistanceToScreenCentreSquared = distanceToCentreSquared;
			_isTargetInLockArea = isTargetInLockedArea;
			_hasLineOfSight = hasLineOfSight;
		}
		else
		{
			_inactiveMarkers.Add(marker);
		}
	}

	private void UpdateLineOfSight(bool hasLOS, HUDMarker marker)
	{
		var isEnemy = hasLOS && marker != null && marker.IsEnemy;
		OnTargetInSightChanged(new LineOfSightInfo
		{
			isEnemyInSight = isEnemy,
			isLockedOn = IsLockedOn
		});
	}

	private void LateUpdate()
	{
		UpdateLineOfSight(_hasLineOfSight, _markerClosestToScreenCentre);

		if (!IsLockedOn || _dropLockOn)
		{
			foreach (var marker in _inactiveMarkers)
			{
				if (marker != null) { marker.DisengageLockOn(); }
			}

			if (_markerClosestToScreenCentre != null)
			{
				if (_dropLockOn)
				{
					_markerClosestToScreenCentre.DisengageLockOn();
					LockedOnHUDMarker = null;
				}
				else
				{
					if (_markerClosestToScreenCentre.EngageLockOnAndCheckIfLockedOn(_isTargetInLockArea))
					{
						LockedOnHUDMarker = _markerClosestToScreenCentre;
					}
				}
			}
		}

		_markerClosestToScreenCentre = null;
		_inactiveMarkers.Clear();
		_shortestDistanceToScreenCentreSquared = 10f;
		_dropLockOn = false;

		_hasLineOfSight = false;
	}
}
