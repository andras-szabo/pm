using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerConditionType
{
	None,
	Always,
	Distance
}

/// <summary>
/// A Point of Interest is a GameObject that can show up on the player's HUD as a marker,
/// after certain conditions are met.
/// </summary>
public class PointOfInterest : MonoWithCachedTransform
{
	public TriggerConditionType condition;
	public int conditionParam;

	public TriggerConditionType destroyCondition;
	public int destroyConditionParam;

	public Color color;
	public string label;
	public bool isLockable;

	private bool _hasShownUpAsHUDMarker;
	private ITriggerCondition _triggerCondition;
	private ITriggerCondition _destroyCondition;

	private void Awake()
	{
		GenerateConditions();
	}

	private void Update()
	{
		if (!_hasShownUpAsHUDMarker && _triggerCondition != null && _triggerCondition.IsSatisfied())
		{
			ShowUpAsHUDMarker();
		}

		if (_hasShownUpAsHUDMarker && _destroyCondition != null && _destroyCondition.IsSatisfied())
		{
			Destroy(gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		if (_triggerCondition != null)
		{
			_triggerCondition.OnDrawGizmos();
		}
	}

	private void GenerateConditions()
	{
		_triggerCondition = TriggerConditionFactory.Create(condition, CachedTransform, conditionParam);
		_destroyCondition = TriggerConditionFactory.Create(destroyCondition, CachedTransform, destroyConditionParam);
	}

	private void ShowUpAsHUDMarker()
	{
		var hud = GameController.TryGetManager<IHUDManager>();
		if (hud != null)
		{
			hud.SpawnMarker(CachedTransform, color, label, isLockable);
			_hasShownUpAsHUDMarker = true;
		}
	}
}
