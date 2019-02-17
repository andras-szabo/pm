using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Compass : MonoBehaviour
{
	[Tooltip("Probably a camera whose orientation you'd like to track.")]
	public Transform targetTransform;

	[Tooltip("Markers for the cardinal directions, clockwise, starting at 0 degrees for north.")]
	public GameObject[] markers;

	[Range(45f, 359f)] public float FOV = 60f;
	[Range(0f, 1f)] public float curvature = 1f;

	public CustomCompassMarker customMarkerPrefab;

	private Vector2 north = new Vector2(0f, 1f);

	private RectTransform _cachedRectTransform;
	private RectTransform CachedRectTransform { get { return _cachedRectTransform ?? (_cachedRectTransform = GetComponent<RectTransform>()); } }
	private Vector3[] _tempCornersArray = new Vector3[4];

	private List<CustomCompassMarker> _customMarkers = new List<CustomCompassMarker>();

	private Vector3 _topLeftCorner;
	private Vector3 _bottomRightCorner;
	private Vector3 _compassDimensions;
	private float _markerVerticalOffset;
	private Vector2 _previousLookDirection;
	private float _previousFOV;
	private float _previousCurvature;

	private void Start()
	{
		RecalculateDimensions();
	}

	private void Update()
	{
		// If we have at least one custom marker, we need to refresh -
		// who knows where they moved between frames.
		var needsToRefresh = _customMarkers.Count > 0;

		if (CachedRectTransform.hasChanged || _previousCurvature != curvature)
		{
			RecalculateDimensions();
			needsToRefresh = true;
		}

		var lookDirection = new Vector2(targetTransform.forward.x, targetTransform.forward.z);

		needsToRefresh |= (_previousFOV != FOV);

		if (!needsToRefresh &&
			Mathf.Approximately(lookDirection.x, _previousLookDirection.x) &&
			Mathf.Approximately(lookDirection.y, _previousLookDirection.y))
		{
			return;
		}
		else
		{
			_previousLookDirection = lookDirection;
			_previousCurvature = curvature;
			_previousFOV = FOV;
		}

		var angleToNorth = Vector2.SignedAngle(north, lookDirection);

		if (angleToNorth < 0) { angleToNorth *= -1f; }
		else { angleToNorth = 360f - angleToNorth; }

		var fovRight = (angleToNorth + FOV / 2f) % 360f;
		var fovLeft = (angleToNorth - FOV / 2f);
		if (fovLeft < 0f) { fovLeft = 360f + fovLeft; }
		if (fovRight < fovLeft) { fovRight += 360f; }

		UpdateCardinalDirectionMarkers(fovLeft, fovRight);
		UpdateCustomMarkers(lookDirection);
	}

	private void UpdateCustomMarkers(Vector2 lookDirection)
	{
		var shouldCleanup = false;
		foreach (var marker in _customMarkers)
		{
			if (marker.HUDMarker != null && marker.HUDMarker.Target != null)
			{
				var toTarget = marker.HUDMarker.Target.position - targetTransform.position;
				var angle = Vector2.SignedAngle(lookDirection, new Vector2(toTarget.x, toTarget.z));

				var markerRelativeHalfPosition = angle / (FOV / 2);
				var markerRelativePosition = Mathf.Clamp01(0.5f - markerRelativeHalfPosition);

				var x = _topLeftCorner.x + markerRelativePosition * _compassDimensions.x;
				var y = _markerVerticalOffset - (_compassDimensions.y * curvature * Mathf.Sin(markerRelativePosition * Mathf.PI));

				marker.transform.position = new Vector3(x, y);
			}
			else
			{
				Destroy(marker.gameObject);
				shouldCleanup = true;
			}
		}

		if (shouldCleanup)
		{
			_customMarkers.RemoveAll(marker => marker == null || 
								     marker.HUDMarker == null || 
									 marker.HUDMarker.Target == null);
		}
	}

	public void AddCustomHUDMarker(HUDMarker marker)
	{
		var newMarker = Instantiate<CustomCompassMarker>(customMarkerPrefab, parent: this.transform);
		newMarker.Setup(marker);
		_customMarkers.Add(newMarker);
	}

	private void UpdateCardinalDirectionMarkers(float fovLeftAngle, float fovRightAngle)
	{
		for (int i = 0; i < markers.Length; ++i)
		{
			var direction = i * (360f / markers.Length);
			if (Mathf.Abs(fovRightAngle - direction) > 360f)
			{
				direction += 360f;
			}

			// If the angle marked by the current direction marker is between
			// the angles marked by field-of-view-left and field-of-view-right,
			// then it's actually visible on the compass.

			if (fovLeftAngle < direction && direction < fovRightAngle)
			{
				markers[i].gameObject.SetActive(true);
				var markerRelativePosition = (direction - fovLeftAngle) / FOV;

				var x = _topLeftCorner.x + markerRelativePosition * _compassDimensions.x;
				var y = _markerVerticalOffset - (_compassDimensions.y * curvature * Mathf.Sin(markerRelativePosition * Mathf.PI));

				markers[i].transform.position = new Vector3(x, y);
			}
			else
			{
				markers[i].gameObject.SetActive(false);
			}
		}
	}

	private void RecalculateDimensions()
	{
		CachedRectTransform.GetWorldCorners(_tempCornersArray);

		_topLeftCorner = _tempCornersArray[1];
		_bottomRightCorner = _tempCornersArray[3];

		_compassDimensions = _bottomRightCorner - _topLeftCorner;
		_markerVerticalOffset = _bottomRightCorner.y - (1f - curvature) * _compassDimensions.y / 2f;

		CachedRectTransform.hasChanged = false;
	}
}
