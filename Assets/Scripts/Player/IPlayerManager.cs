using System;
using UnityEngine;

public interface IPlayerManager : IManager
{
	event Action<HPInfo> OnHealthChanged;

	Rigidbody Rigidbody { get; }
	Transform Transform { get; }

	HPInfo GetHPInfo();
	ProjectileLauncher GetLauncherWithTag(int weaponTag);
}
