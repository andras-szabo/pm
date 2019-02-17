using UnityEngine;

public class PoolUser : MonoBehaviour 
{
	private IPoolManager _poolManager;

	private void Start()
	{
		_poolManager = GameController.TryGetManager<IPoolManager>();			
	}

	public void Despawn()
	{
		if (_poolManager != null)
		{
			_poolManager.Despawn(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
