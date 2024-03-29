﻿using UnityEngine;
using UnityEngine.UI;

public class HUDMarker : MonoWithCachedTransform
{
	private Vector2 OUT_OF_SIGHT = new Vector2(10f, 10f);
	
	public Image lockReticle;
	public Image marker;
	public Text label;

	public bool IsEnemy { get { return _isLockable; } }
	public Transform Target { get { return _target; } }
	public Transform LockedOnTarget { get { return IsLockedOn ? _target : null; } }
	public Color Color { get { return marker.color; } }

	public bool IsLockedOn { get; private set; }

	[Range(0f, 0.2f)]
	public float minEdgeFromScreenH = 0.05f;

	[Range(0f, 0.2f)]
	public float minEdgeFromScreenV = 0.05f;

	private Transform _target;
	private Camera _viewCamera;
	private float _halfWidth;
	private float _halfHeight;
	private Transform _camTransform;
	private IHUDManager _hudManager;
	private ILockOnManager _lockOnManager;
	private float _elapsedTimeLocking = 0f;
	private Vector2 _screenCoords;
	private bool _wasLocking;
	private float _secondsRequiredForLockOn;
	private bool _isLockable;
	private Vector2 _clampEdge;

	private Vector3 _lockOnReticleDefaultScale;
	private Vector2 _lockTolerance;
	private float _maxLockOnDistance;

	private int _LOSlayermask;

	public void Setup(Camera viewCamera, Transform target, Vector2 screenHalfDimensions,
				      Color color, string label, bool isLockable, IHUDManager hudManager)
	{
		this._target = target;
		_viewCamera = viewCamera;
		_camTransform = _viewCamera.transform;

		_halfWidth = screenHalfDimensions.x;
		_halfHeight = screenHalfDimensions.y;

		_hudManager = hudManager;
		_lockOnManager = hudManager.LockOnManager;
		_secondsRequiredForLockOn = _lockOnManager.SecondsRequiredForLockOn;
		_lockTolerance = _lockOnManager.LockTolerance;
		_maxLockOnDistance = _lockOnManager.MaxLockOnDistance;
		_lockOnReticleDefaultScale = lockReticle.transform.localScale;
		_isLockable = isLockable;

		this.marker.color = color;
		this.label.text = label;
		this.label.color = color;

		UpdateLockOnVisuals(false);

		_LOSlayermask = LayerMask.GetMask("EnemyBullet"); 
		_LOSlayermask = ~_LOSlayermask;

		_clampEdge = new Vector2(1f - minEdgeFromScreenH, 1f - minEdgeFromScreenV);

		CachedTransform.SetAsFirstSibling();
	}

	private void Update()
	{
		if (_target == null)
		{
			_hudManager.RemoveMarker(this);
			return;
		}

		var camToTarget = (_target.position - _camTransform.position).normalized;
		float dotForwardVsTarget;
		if (IsBehindCamera(camToTarget, out dotForwardVsTarget))
		{
			ShowForTargetBehind(camToTarget, dotForwardVsTarget);
		}
		else
		{
			ShowVisibleTargetInScreenSpace();
		}

		if (_isLockable)
		{
			UpdateLockStatus();
		}
	}

	private void OnDestroy()
	{
		_lockOnManager.RemoveMarker(this);
	}

	private void UpdateLockStatus()
	{
		if (_lockOnManager != null)
		{
			float distanceToCentreSquared;
			bool hasLineOfSight;
			var isTargetInLockedArea = IsTargetInLockArea(out distanceToCentreSquared, out hasLineOfSight);
			_lockOnManager.UpdateLockStatus(this, isTargetInLockedArea, distanceToCentreSquared, hasLineOfSight);
		}
	}

	public bool EngageLockOnAndCheckIfLockedOn(bool targetInLockArea)
	{
		_wasLocking = true;
		var elapsedTimeMultiplier = targetInLockArea ? 1f : -1f;
		_elapsedTimeLocking += (Time.deltaTime * elapsedTimeMultiplier);
		_elapsedTimeLocking = Mathf.Max(0f, _elapsedTimeLocking);
		
		UpdateLockOnVisuals(true, _elapsedTimeLocking / _secondsRequiredForLockOn);

		IsLockedOn = _elapsedTimeLocking >= _secondsRequiredForLockOn;
		return IsLockedOn;
	}

	public void DisengageLockOn()
	{
		IsLockedOn = false;
		float deltaT = _wasLocking ? 0.5f : Time.deltaTime;
		_wasLocking = false;
		_elapsedTimeLocking = Mathf.Max(0f, _elapsedTimeLocking - deltaT);
		UpdateLockOnVisuals(false);
	}

	private void UpdateLockOnVisuals(bool isLockOnEngaged, float lockOnRatio = 0f)
	{
		//TODO let's have a component that does this, but for now:
		lockReticle.gameObject.SetActive(isLockOnEngaged);

		lockReticle.transform.localScale = _lockOnReticleDefaultScale * (2f - lockOnRatio);
		lockReticle.fillAmount = lockOnRatio;
	}

	private bool IsTargetInLockArea(out float distanceToScreenCentreSquared, out bool hasLineOfSight)
	{
		distanceToScreenCentreSquared = _screenCoords.sqrMagnitude;

		var isInLockArea = Mathf.Abs(_screenCoords.x) < _lockTolerance.x &&
						   Mathf.Abs(_screenCoords.y) < _lockTolerance.y;

		hasLineOfSight = isInLockArea && HasLineOfSight();

		return isInLockArea && hasLineOfSight;
	}

	private bool HasLineOfSight()
	{
		RaycastHit hit;
		if (Physics.Raycast(_camTransform.position, _target.position - _camTransform.position, 
							out hit, _maxLockOnDistance, _LOSlayermask))
		{
			return hit.transform == _target;
		}

		return false;
	}

	private void ShowVisibleTargetInScreenSpace()
	{
		var view = _viewCamera.worldToCameraMatrix;
		var proj = _viewCamera.projectionMatrix;

		var homogeneousTargetCoords = new Vector4(_target.position.x, _target.position.y, _target.position.z, 1f);
		var clipSpaceCoords = proj * view * homogeneousTargetCoords;
		_screenCoords = new Vector2(clipSpaceCoords.x, clipSpaceCoords.y) / clipSpaceCoords.w;

		var clampedX = Mathf.Clamp(_screenCoords.x, -_clampEdge.x, _clampEdge.x);
		var clampedY = Mathf.Clamp(_screenCoords.y, -_clampEdge.y, _clampEdge.y);

		CachedTransform.localPosition = new Vector3(clampedX * _halfWidth, clampedY * _halfHeight);
	}

	private void ShowForTargetBehind(Vector3 camToTarget, float dotForwardVsTarget)
	{
		var isOnCamsRightSide = Vector3.Dot(_camTransform.right, camToTarget) > 0f ? 1f : -1f;
		var screenPositionRatio = 1f + dotForwardVsTarget;
		var posX = _halfWidth * isOnCamsRightSide * screenPositionRatio * _clampEdge.x;

		_screenCoords = OUT_OF_SIGHT;

		CachedTransform.localPosition = new Vector3(posX, -_halfHeight * _clampEdge.y);
	}

	private bool IsBehindCamera(Vector3 direction, out float dotProduct)
	{
		dotProduct = Vector3.Dot(_camTransform.forward, direction);
		return dotProduct < 0f;
	}
}
