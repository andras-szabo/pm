using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HUDCustomCameraScreen : MonoBehaviour
{
	public CustomCameraType type;

	private RectTransform _rt;
	private RectTransform CachedRectTransform
	{
		get
		{
			return _rt ?? (_rt = GetComponent<RectTransform>());
		}
	}

	public Vector3[] GetWorldCorners()
	{
		Vector3[] worldCorners = new Vector3[4];
		CachedRectTransform.GetWorldCorners(worldCorners);
		return worldCorners;
	}
}
