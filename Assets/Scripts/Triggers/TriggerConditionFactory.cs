using UnityEngine;

public static class TriggerConditionFactory
{
	public static ITriggerCondition Create(TriggerConditionType type, Transform ownTransform, int param)
	{
		switch (type)
		{
			case TriggerConditionType.Always: return new AlwaysTrueCondition();
			case TriggerConditionType.Distance: return new DistanceCondition(ownTransform, param);
			default:
				return null;
		}
	}
}

public class AlwaysTrueCondition : ITriggerCondition
{
	public bool IsSatisfied()
	{
		return true;
	}

	public void OnDrawGizmos()
	{
		return;
	}
}

public class DistanceCondition : ITriggerCondition
{
	private Transform _transform;
	private float _distanceSquared;

	private IPlayerManager _pm;
	private IPlayerManager PM
	{
		get
		{
			return _pm ?? (_pm = GameController.TryGetManager<IPlayerManager>());
		}
	}

	private Transform _playerTransform;
	private Transform PlayerTransform
	{
		get
		{
			return _playerTransform ?? (_playerTransform = PM.Transform); 
		}
	}

	public DistanceCondition(Transform ownTransform, int param)
	{
		_transform = ownTransform;
		_distanceSquared = param * param;
	}

	public bool IsSatisfied()
	{

		return PlayerTransform != null && Vector3.SqrMagnitude(PlayerTransform.position - _transform.position) < _distanceSquared;
	}

	public void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(_transform.position, Mathf.Sqrt(_distanceSquared));
		}
	}
}
