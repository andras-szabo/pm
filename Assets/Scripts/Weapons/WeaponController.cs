using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour 
{
	public ProjectileLauncher[] launchers;

	private Dictionary<int, List<ProjectileLauncher>> _launchersByTag = new Dictionary<int, List<ProjectileLauncher>>();
	private List<ProjectileLauncher> _workingList;

	private void Awake()
	{
		foreach (var launcher in launchers)
		{
			var tag = launcher.weaponTag;
			if (!_launchersByTag.ContainsKey(tag))
			{
				_launchersByTag[tag] = new List<ProjectileLauncher>();
			}

			_launchersByTag[tag].Add(launcher);
		}
	}

	public void ExecuteCommand(IWeaponCommand command)
	{
		command.Execute(this);
	}

	public void Shoot(int weaponTag, Transform target)
	{
		if (_launchersByTag.TryGetValue(weaponTag, out _workingList))
		{
			foreach (var launcher in _workingList)
			{
				launcher.TryLaunch(target);
			}
		}
	}
}
