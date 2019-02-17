using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
	private static GameController _instance;

	public static void Cleanup()
	{
		_instance._managers.Clear();
		_instance = null;
	}

	public static bool TryRegister<T>(T manager) where T : IManager
	{
		if (_instance == null || _instance._managers.ContainsKey(typeof(T)))
		{
			return false;
		}

		_instance._managers.Add(typeof(T), manager);
		return true;
	}

	public static T TryGetManager<T>() where T : class, IManager
	{
		IManager manager;

		if (_instance != null && _instance._managers.TryGetValue(typeof(T), out manager))
		{
			return (T)manager;
		}

		return null;
	}

	private Dictionary<System.Type, IManager> _managers = new Dictionary<System.Type, IManager>();

	#region Unity lifecycle of a monosingleton
	private void Awake()
	{
		if (_instance == null)
		{
			InitInstance();
		}
		else
		{
			if (_instance != this)
			{
				Destroy(gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			Cleanup();
		}
	}
	#endregion

	private void InitInstance()
	{
		_instance = this;
		_managers.Add(typeof(ICollisionManager), new CollisionManager());
	}
}

//TODO
//Sep. file
public interface IManager { }
public interface IHUDManager : IManager 
{ 
	ILockOnManager LockOnManager { get; }
	void SpawnMarker(UnityEngine.Transform target, Color color, string label, bool isLockable);
	void RemoveMarker(HUDMarker marker);
	Rect GetPixelRectForCamViewport(CustomCameraType type);
}
