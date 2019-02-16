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

	public DistanceCondition(Transform ownTransform, int param)
	{
		_transform = ownTransform;
		_distanceSquared = param * param;
	}

	public bool IsSatisfied()
	{
		var player = GameController.TryGetPlayerTransform();
		return player != null && Vector3.SqrMagnitude(player.position - _transform.position) < _distanceSquared;
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
