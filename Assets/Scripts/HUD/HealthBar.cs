using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
	public Image healthBar;
	public Text healthLabel;

	[Range(0, 1f)]
	public float startWarningBelow = 0.25f;

	[Range(0.2f, 1f)]
	public float blinkPeriodSeconds = 0.25f;

	public Color fullHealthColor;
	public Color zeroHealthColor;
	public Color warningColor;

	private Color _currentColor;
	private Coroutine _blinkRoutine;

	private void Start()
	{
		ObservePlayer(true);
		HandlePlayerHealthChanged(GameController.TryGetManager<IPlayerManager>().GetHPInfo());
	}

	private void OnDestroy()
	{
		ObservePlayer(false);
	}

	private void HandlePlayerHealthChanged(HPInfo info)
	{
		healthBar.fillAmount = info.RateToFull;
		SetBarColor(info.RateToFull);
		healthLabel.text = string.Format("Health: {0} / {1}", info.current, info.max);
	}

	private void SetBarColor(float rateToFull)
	{
		_currentColor = Color.Lerp(zeroHealthColor, fullHealthColor, rateToFull);

		if (_blinkRoutine == null && rateToFull < startWarningBelow)
		{
			_blinkRoutine = StartCoroutine(BlinkWarningRoutine());
		}

		if (_blinkRoutine == null)
		{
			healthBar.color = _currentColor;
		}
	}

	private IEnumerator BlinkWarningRoutine()
	{
		while (true)
		{
			var currentColor = _currentColor;
			var toColor = warningColor;
			var elapsed = 0f;
			while (elapsed < blinkPeriodSeconds)
			{
				healthBar.color = Color.Lerp(currentColor, toColor, elapsed / blinkPeriodSeconds);
				yield return null;
				elapsed += Time.deltaTime;
			}

			elapsed = 0f;
			currentColor = _currentColor;
			while (elapsed < blinkPeriodSeconds)
			{
				healthBar.color = Color.Lerp(toColor, currentColor, elapsed / blinkPeriodSeconds);
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
	}

	private void ObservePlayer(bool register)
	{
		var playerMgr = GameController.TryGetManager<IPlayerManager>();
		if (playerMgr != null)
		{
			if (register)
			{
				playerMgr.OnHealthChanged += HandlePlayerHealthChanged;
			}
			else
			{
				playerMgr.OnHealthChanged -= HandlePlayerHealthChanged;
			}
		}
	}
}
