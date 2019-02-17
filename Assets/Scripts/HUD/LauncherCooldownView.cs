using UnityEngine;
using UnityEngine.UI;

public class LauncherCooldownView : MonoBehaviour 
{
	public Image cooldownBar;
	public Text cooldownLabel;

	public int weaponTag;
	private ProjectileLauncher _launcherToObserve;

	private float _cooldownRate;

	private void Start()
	{
		_launcherToObserve = GameController.TryGetManager<IPlayerManager>().GetLauncherWithTag(weaponTag);
	}

	private void Update()
	{
		var currentCooldown = _launcherToObserve.CooldownRate;
		if (!Mathf.Approximately(currentCooldown, _cooldownRate))
		{
			_cooldownRate = currentCooldown;
			cooldownBar.fillAmount = _cooldownRate;
			cooldownLabel.gameObject.SetActive(_cooldownRate >= 1f);
		}
	}
}
