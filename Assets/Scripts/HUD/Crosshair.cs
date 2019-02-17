using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour 
{
	public Color lockedOn;
	public Color neutral;
	public Color lookingAtEnemy;

	[Range(0.1f, 2f)]
	public float secondsToFadeColor = 0.2f;

	public Image image;
	public Text lockedOnLabel;

	private void Start()
	{
		var lockOnMgr = GameController.TryGetManager<IHUDManager>().LockOnManager;
		
		lockOnMgr.OnLockOnChanged += HandleLockOnChanged;
		lockOnMgr.OnTargetInSightChanged += HandleTargetInSightChanged;

		image.color = neutral;
		lockedOnLabel.color = lockedOn;
	}

	private void OnDestroy()
	{
		var hud = GameController.TryGetManager<IHUDManager>();
		if (hud != null && hud.LockOnManager != null)
		{
			hud.LockOnManager.OnLockOnChanged -= HandleLockOnChanged;
			hud.LockOnManager.OnTargetInSightChanged -= HandleTargetInSightChanged;
		}
	}

	private void HandleTargetInSightChanged(LineOfSightInfo los)
	{
		StopAllCoroutines();
		StartCoroutine(FadeColorRoutine(isLookingAtEnemy: los.isEnemyInSight));
	}

	private void HandleLockOnChanged(Transform target)
	{
		lockedOnLabel.gameObject.SetActive(target != null);
	}

	private IEnumerator FadeColorRoutine(bool isLookingAtEnemy)
	{
		var startColor = image.color;
		var endColor = isLookingAtEnemy ? lookingAtEnemy : neutral;
		var elapsedSeconds = 0f;

		while (elapsedSeconds < secondsToFadeColor)
		{
			image.color = Color.Lerp(startColor, endColor, elapsedSeconds / secondsToFadeColor);
			yield return null;
			elapsedSeconds += Time.deltaTime;
		}
	}
}
