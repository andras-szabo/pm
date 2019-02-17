using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoverThruster : MonoBehaviour
{
	public struct HoverInput
	{
		public readonly float vertical;
		public readonly float horizontal;
		public readonly bool jump;

		public HoverInput(float vert, float hori, bool jmp)
		{
			vertical = vert;
			horizontal = hori;
			jump = jmp;
		}
	};

	[Range(1f, 10f)] public float hoverHeight;
	[Range(0f, 50f)] public float speed;
	[Range(0f, 50f)] public float turnSensitivity;
	[Range(10f, 1000f)] public float jumpStrength;
	[Range(1f, 150f)] public float correctiveForce;

	public bool strafe;

	private Rigidbody _rb;
	private Rigidbody CachedRigidbody { get { return _rb ?? (_rb = GetComponent<Rigidbody>()); } }

	private Transform _transform;
	private Transform CachedTransform {	get { return _transform ?? (_transform = this.transform); } }

	private float _defaultForce;
	private HoverInput _input;

	private float _flipCorrectionSeconds = 0.5f;
	private float _elapsedSinceFlipCorrectionStart = 0f;

	private void Awake()
	{
		_defaultForce = -Physics.gravity.y * CachedRigidbody.mass;
	}

	private void FixedUpdate()
	{
		ApplyHover(_input);
		ApplyFlipCorrection();
	}

	private void Update()
	{
		ReadInput();
	}

	public float zTolerance = 2f;

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

	private void ReadInput()
	{
		_input = new HoverInput(vert: Input.GetAxis("Vertical"),
								hori: Input.GetAxis("Horizontal"),
								jmp: Input.GetKeyDown(KeyCode.Space));
	}

	public void ApplyHover(HoverInput hoverInput)
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
