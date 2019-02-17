using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - pooling
public class TTL : MonoBehaviour 
{
	public bool autoSetup;
	public float timeToLiveSeconds;

	private float _timeToLive;
	private float _elapsedTime;
	private bool _isSetup;

	private void Awake()
	{
		if (autoSetup)
		{
			Setup(timeToLiveSeconds);
		}
	}

	public void Setup(float timeToLiveSeconds)
	{
		_isSetup = true;
		_elapsedTime = 0f;
		_timeToLive = Mathf.Max(0f, timeToLiveSeconds);
	}

	private void Update()
	{
		if (_isSetup)
		{
			_elapsedTime += Time.deltaTime;
			if (_elapsedTime >= _timeToLive)
			{
				UnityEngine.Object.Destroy(this.gameObject);
			}
		}
	}
}
