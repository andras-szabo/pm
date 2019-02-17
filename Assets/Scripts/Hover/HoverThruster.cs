using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoverThruster : MonoBehaviour
{
	public struct HoverInput
	{
		public float vertical;
		public float horizontal;
		public bool jump;

		public HoverInput(float vert, float hori, bool jmp)
		{
			vertical = vert;
			horizontal = hori;
			jump = jmp;
		}
	};

	[Range(1f, 20f)] public float hoverHeight;
	[Range(0f, 50f)] public float speed;
	[Range(0f, 50f)] public float turnSensitivity;
	[Range(10f, 1000f)] public float jumpStrength;
	[Range(1f, 150f)] public float correctiveForce;

	public bool strafe;
	public float zTolerance = 2f;

	private Rigidbody _rb;
	public Rigidbody CachedRigidbody { get { return _rb ?? (_rb = GetComponent<Rigidbody>()); } }

	private Transform _transform;
	private Transform CachedTransform {	get { return _transform ?? (_transform = this.transform); } }

	private float _defaultForce;

	private float _flipCorrectionSeconds = 0.5f;
	private float _elapsedSinceFlipCorrectionStart = 0f;

	private HoverInput _input;

	public void SetInput(HoverInput inp)
	{
		_input = inp;
	}

	public void SetInput(float vert, float hori, bool jmp)
	{
		_input.vertical = vert;
		_input.horizontal = hori;
		_input.jump = jmp;
	}

	private void Awake()
	{
		_defaultForce = -Physics.gravity.y * CachedRigidbody.mass;
	}

	private void FixedUpdate()
	{
		ApplyHover(_input);
		ApplyFlipCorrection();
	}

	private void ApplyFlipCorrection()
	{
		var currentRotation = CachedTransform.rotation;
		var asEuler = currentRotation.eulerAngles;

		if (Mathf.Abs(asEuler.z) > zTolerance || Mathf.Abs(asEuler.x) > zTolerance)
		{
			_elapsedSinceFlipCorrectionStart += Time.deltaTime;
			var ideal = Quaternion.Euler(0f, asEuler.y, 0f);

			CachedTransform.rotation = Quaternion.Slerp(currentRotation, ideal,
				_elapsedSinceFlipCorrectionStart / _flipCorrectionSeconds);
		}
		else
		{
			_elapsedSinceFlipCorrectionStart = 0f;
		}
	}

	private void ApplyHover(HoverInput hoverInput)
	{
		var ray = new Ray(CachedTransform.position, -CachedTransform.up);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, hoverHeight))
		{
			var force = _defaultForce + ( (hoverHeight - hit.distance) / hoverHeight * correctiveForce);
			CachedRigidbody.AddForce(Vector3.up * force, ForceMode.Force);
		}

		CachedRigidbody.AddRelativeForce(0f, 0f, hoverInput.vertical * speed, ForceMode.Acceleration);

		if (strafe)
		{
			CachedRigidbody.AddRelativeForce(hoverInput.horizontal * speed, 0f, 0f, ForceMode.Acceleration);
		}
		else
		{
			CachedRigidbody.AddRelativeTorque(0f, hoverInput.horizontal * turnSensitivity, 0f, ForceMode.Acceleration);
		}

		if (hoverInput.jump)
		{
			CachedRigidbody.AddForce(Vector3.up * jumpStrength, ForceMode.Acceleration);
		}
	}
}
