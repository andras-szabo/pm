using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(HoverThruster))]
public class PlayerManager : MonoWithCachedTransform, IPlayerManager
{
	private WeaponController _weaponController;
	private HoverThruster _hoverThruster;

	private HoverThruster.HoverInput _hoverInput;

	public Rigidbody Rigidbody { get { return _hoverThruster.CachedRigidbody; } }
	public Transform Transform { get { return CachedTransform; } }

	private void Awake()
	{
		GameController.TryRegister<IPlayerManager>(this);
		_weaponController = GetComponent<WeaponController>();
		_hoverThruster = GetComponent<HoverThruster>();
	}

	private void Update()
	{
		ProcessMovementInput();
		ProcessWeaponInput();
	}

	private void ProcessWeaponInput()
	{
		if (Input.GetMouseButtonDown(1))
		{
			var target = GameController.TryGetManager<IHUDManager>().LockOnManager.LockedOnTransform;

			if (target != null)
			{
				_weaponController.Shoot(1, target);
			}
		}

		if (Input.GetMouseButton(0))
		{
			_weaponController.Shoot(0, null);
		}
	}

	private void ProcessMovementInput()
	{
		_hoverThruster.SetInput(vert: Input.GetAxis("Vertical"),
								hori: Input.GetAxis("Horizontal"),
								 jmp: Input.GetKeyDown(KeyCode.Space));
	}
}
