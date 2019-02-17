using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RawImage))]
public class HUDCustomCameraScreen : MonoBehaviour
{
	public CustomCameraType type;

	private RawImage _rawImage;
	private Coroutine _showNoiseAndSwitchOffRoutine;
	private WaitForSeconds _noiseDuration = new WaitForSeconds(0.5f);

	private RectTransform _rt;
	private RectTransform CachedRectTransform
	{
		get
		{
			return _rt ?? (_rt = GetComponent<RectTransform>());
		}
	}

	private void Start()
	{
		_rawImage = GetComponent<RawImage>();
		_rawImage.enabled = false;
	}

	public Vector3[] GetWorldCorners()
	{
		Vector3[] worldCorners = new Vector3[4];
		CachedRectTransform.GetWorldCorners(worldCorners);
		return worldCorners;
	}

	public void Toggle(bool state)
	{
		if (state)
		{
			if (_showNoiseAndSwitchOffRoutine != null)
			{
				StopCoroutine(_showNoiseAndSwitchOffRoutine);
			}

			_rawImage.enabled = false;
		}
		else
		{
			ShowNoiseAndSwitchOff();
		}
	}

	private void ShowNoiseAndSwitchOff()
	{
		if (_showNoiseAndSwitchOffRoutine != null)
		{
			StopCoroutine(_showNoiseAndSwitchOffRoutine);
		}

		_showNoiseAndSwitchOffRoutine = StartCoroutine(ShowNoiseAndSwitchOffRoutine());
	}

	private IEnumerator ShowNoiseAndSwitchOffRoutine()
	{
		_rawImage.enabled = true;
		yield return _noiseDuration;
		_rawImage.enabled = false;
		_showNoiseAndSwitchOffRoutine = null;
	}
}
