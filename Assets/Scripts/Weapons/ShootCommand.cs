using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShootCommand : IWeaponCommand
{
	public readonly int weaponTag;
	public readonly Transform target;

	public ShootCommand(int weaponTag, Transform target)
	{
		this.weaponTag = weaponTag;
		this.target = target;
	}

	public void Execute(WeaponController controller)
	{
		controller.Shoot(weaponTag, target);
	}
}
