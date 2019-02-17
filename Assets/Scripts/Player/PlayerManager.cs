using System;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(HoverThruster))]
[RequireComponent(typeof(HP))]
public class PlayerManager : MonoWithCachedTransform, IPlayerManager
{
	public const int MOUSE_BUTTON_WEAPON_TAG_0 = 0;
	public const int MOUSE_BUTTON_WEAPON_TAG_1 = 1;

	public CameraRotator[] camRotators;

	public event Action<HPInfo> OnHealthChanged;
	
	public Rigidbody Rigidbody { get { return _hoverThruster.CachedRigidbody; } }
	public Transform Transform { get { return CachedTransform; } }

	private WeaponController _weaponController;
	private HoverThruster _hoverThruster;
	private HP _hp;

	private Transform _lockedOnTarget;

	private void Awake()
	{
		GameController.TryRegister<IPlayerManager>(this);
		_weaponController = GetComponent<WeaponController>();
		_hoverThruster = GetComponent<HoverThruster>();
		_hp = GetComponent<HP>();

		_hp.OnHitPointsChanged += HandleHPChanged;
	}

	private void Start()
	{
		var lockOnManager = GameController.TryGetManager<IHUDManager>().LockOnManager;
		lockOnManager.OnLockOnChanged += HandleLockOnChanged;
	}

	private void OnDestroy()
	{
		var hud = GameController.TryGetManager<IHUDManager>();
		if (hud != null)
		{
			var lockOnManager = hud.LockOnManager;
			if (lockOnManager != null)
			{
				lockOnManager.OnLockOnChanged -= HandleLockOnChanged;
			}
		}
	}

	public HPInfo GetHPInfo()
	{
		return _hp.GetHPInfo();
	}

	private void HandleLockOnChanged(Transform newTarget)
	{
		_lockedOnTarget = newTarget;
	}

	private void HandleHPChanged(HPInfo hpInfo)
	{
		OnHealthChanged?.Invoke(hpInfo);
	}

	private void Update()
	{
		ProcessMovementInput();
		ProcessWeaponInput();
	}

	private void LateUpdate()
	{
		ProcessCameraRotationInput();
	}

	private void ProcessCameraRotationInput()
	{
		var v = Input.GetAxis("Mouse Y");
		var h = Input.GetAxis("Mouse X");

		foreach (var camRotator in camRotators)
		{
			camRotator.ApplyInput(v, h);
		}
	}

	private void ProcessWeaponInput()
	{
		if (Input.GetMouseButtonDown(MOUSE_BUTTON_WEAPON_TAG_1))
		{
			if (_lockedOnTarget != null)
			{
				_weaponController.Shoot(1, _lockedOnTarget);
			}
		}

		if (Input.GetMouseButton(MOUSE_BUTTON_WEAPON_TAG_0))
		{
			_weaponController.Shoot(0, null);
		}
	}

	private void ProcessMovementInput()
	{
		_hoverThruster.SetInput(vert: Input.GetAxis("Vertical"),
								hori: Input.GetAxis("Horizontal"),
								 jmp: Input.GetKey(KeyCode.Space));
	}
}
