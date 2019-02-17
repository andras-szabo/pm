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

	//TODO - swhere else?
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	//TODO clamp at least pitch
	private void LateUpdate()
	{
		var pitch = ignorePitch ? 0f : Input.GetAxis("Mouse Y") * (invertMouseVertical ? 1f : -1f);
		var yaw = ignoreYaw ? 0f : Input.GetAxis("Mouse X");

		var rotationSoFar = CachedTransform.rotation.eulerAngles;
		var rotationDelta = new Vector3(pitch, yaw, 0f) * rotSensitivity * Time.deltaTime;
		CachedTransform.rotation = Quaternion.Euler(rotationDelta + rotationSoFar);
	}
}
