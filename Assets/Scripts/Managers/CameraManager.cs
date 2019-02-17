using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraManager
{
	private Dictionary<CustomCameraType, List<CustomCameraContainer>> _customCams = new Dictionary<CustomCameraType, List<CustomCameraContainer>>();
	private Dictionary<CustomCameraType, CustomCameraContainer> _activeCustomCams = new Dictionary<CustomCameraType, CustomCameraContainer>();

	private List<CustomCameraContainer> _workingCamList;
	private IHUDManager _hud;

	private void Awake()
	{
		GameController.TryRegister<ICameraManager>(this);	
	}

	private void Start()
	{
		_hud = GameController.TryGetManager<IHUDManager>();	
	}

	public Rect RegisterAndGetViewportPixelRect(CustomCameraContainer camContainer)
	{
		var type = camContainer.type;
		if (!_customCams.ContainsKey(type))
		{
			_customCams[type] = new List<CustomCameraContainer> { camContainer };
		}
		else
		{
			_customCams[type].Add(camContainer);
		}

		TrySetAsActiveCamera(camContainer);

		return _hud.GetPixelRectForCamViewport(type);
	}

	public void Unregister(CustomCameraContainer camContainer)
	{
		var type = camContainer.type;
		if (_customCams.ContainsKey(type)) { _customCams[type].Remove(camContainer); }
		if (_activeCustomCams.ContainsKey(type)) { _activeCustomCams[type] = null; }

		if (!TrySetNextActiveCamera(type))
		{
			_hud.ToggleCustomCameraScreen(type, false);
		}
	}

	private bool TrySetNextActiveCamera(CustomCameraType type)
	{
		if (_customCams.TryGetValue(type, out _workingCamList))
		{
			if (_workingCamList.Count > 0)
			{
				TrySetAsActiveCamera(_workingCamList[0]);
				return true;
			}
		}

		return false;
	}

	private void TrySetAsActiveCamera(CustomCameraContainer camContainer)
	{
		CustomCameraContainer previousActiveCam;

		if (_activeCustomCams.TryGetValue(camContainer.type, out previousActiveCam))
		{
			if (previousActiveCam != null) { previousActiveCam.Hide(); }
		}

		_hud.ToggleCustomCameraScreen(camContainer.type, true);

		_activeCustomCams[camContainer.type] = camContainer;
		camContainer.Show();
	}
}
