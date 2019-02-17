using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class HUDCam : MonoBehaviour 
{
	public Transform target;
	public Transform widget;

	public Canvas canvas;
	public CanvasScaler cscaler;

	private Camera _cachedCam;
	private Camera CachedCam
	{
		get
		{
			return _cachedCam ?? (_cachedCam = GetComponent<Camera>());
		}
	}

	private Transform _cachedTransform;
	private Transform CachedTransform
	{
		get
		{
			return _cachedTransform ?? (_cachedTransform = this.transform);
		}
	}



	private void Start()
	{
		print(string.Format("Screen width: {0}, canvas width: {1}", Screen.width, canvas.pixelRect.width));

		var aspr = (float)Screen.width / (float)Screen.height;
		var refres = cscaler.referenceResolution.x / cscaler.referenceResolution.y;

		print(string.Format("Refres/aspratio: {0}, asp/refres: {1}",
							refres / aspr, aspr / refres));

		print(string.Format("screen/ref: {0}, ref/screen: {1}",
							(float)Screen.width / cscaler.referenceResolution.x,
							cscaler.referenceResolution.x / (float)Screen.width));
	}

		private bool IsBehindCamera(Vector3 direction, out float dotProduct)
	{
		dotProduct = Vector3.Dot(CachedTransform.forward, direction);
		return dotProduct < 0f;
	}

	private void Update()
	{
		//TODO - cache, only update when needed	
		// + note about log space
		var logWidth = Mathf.Log((float)Screen.width / cscaler.referenceResolution.x, 2.718f);
		var logHeight = Mathf.Log((float)Screen.height / cscaler.referenceResolution.y, 2.718f);
		var logWeightedAvg = Mathf.Lerp(logWidth, logHeight, cscaler.matchWidthOrHeight);
		var resolutionFactor = Mathf.Pow(2.718f, logWeightedAvg);

		var halfWidth = canvas.pixelRect.width / 2f / resolutionFactor;
		var halfHeight = canvas.pixelRect.height / 2f / resolutionFactor;

		var camToTarget = (target.position - transform.position).normalized;
		//TODO out
		float dot;
		if (IsBehindCamera(camToTarget, out dot))
		{
			var isOnCamsRightSide = Vector3.Dot(transform.right, camToTarget) > 0f ? 1f : -1f;
			var screenPositionRatio = 1f + dot;
			var posX = halfWidth * isOnCamsRightSide * screenPositionRatio;
			widget.localPosition = new Vector3(posX, -halfHeight);
			return;
		}

		var view = CachedCam.worldToCameraMatrix;
		var proj = CachedCam.projectionMatrix;

		var homogeneousTargetCoords = new Vector4(target.position.x, target.position.y, target.position.z, 1f);
		var clipCoords = proj * view * homogeneousTargetCoords;
		var screenCoords = new Vector2(clipCoords.x / clipCoords.w, clipCoords.y / clipCoords.w);

		var clampedX = Mathf.Clamp(screenCoords.x, -1f, 1f);
		var clampedY = Mathf.Clamp(screenCoords.y, -1f, 1f);

		widget.localPosition = new Vector3(clampedX * halfWidth,
										   clampedY * halfHeight);
	}

}
