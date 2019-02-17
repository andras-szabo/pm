using UnityEngine;

public class CameraRotator : MonoBehaviour
{
	[Range(1f, 500f)] public float rotSensitivity = 100f;
	public bool invertMouseVertical = true;

	public bool ignoreYaw;
	public bool ignorePitch;

	private Transform _cachedTransform;
	private Transform CachedTransform
	{
		get { return _cachedTransform ?? (_cachedTransform = this.transform); }
	}

	//TODO clamp at least pitch
	public void ApplyInput(float inputV, float inputH)
	{
		var pitch = ignorePitch ? 0f : inputV * (invertMouseVertical ? 1f : -1f);
		var yaw = ignoreYaw ? 0f : inputH;

		var rotationSoFar = CachedTransform.rotation.eulerAngles;
		var rotationDelta = new Vector3(pitch, yaw, 0f) * rotSensitivity * Time.deltaTime;
		CachedTransform.rotation = Quaternion.Euler(rotationDelta + rotationSoFar);
	}
}
