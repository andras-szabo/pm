using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoverThruster))]
public class FlyingMeanie : MonoWithCachedTransform 
{
	public float desiredDistanceToPlayer = 5f;

	private HoverThruster _hoverThruster;
	private Transform _player;

	private void Awake()
	{
		_hoverThruster = GetComponent<HoverThruster>();
	}

	private void Start()
	{
		_player = GameController.TryGetManager<IPlayerManager>().Transform;
	}

	private void Update()
	{
		ProcessMovementInput();
	}

	private void ProcessMovementInput()
	{
		var horizontalInputToTurnTowardsPlayer = CalculateHorizontalInput();
		var verticalInputToApproachPlayer = CalculateAccelerationInput();
		_hoverThruster.SetInput(vert: verticalInputToApproachPlayer, hori: horizontalInputToTurnTowardsPlayer, jmp: false);
	}

	private float CalculateAccelerationInput()
	{
		var playerPos = _player.position;
		var myPos = CachedTransform.position;
		var currentDistance = Mathf.Sqrt(Mathf.Pow(playerPos.x - myPos.x, 2f) + Mathf.Pow(playerPos.z - myPos.z, 2f));

		return currentDistance > desiredDistanceToPlayer ? Mathf.Min(1f, (currentDistance - desiredDistanceToPlayer) / currentDistance)
												 : Mathf.Max(-1f, -(desiredDistanceToPlayer - currentDistance) / currentDistance);
	}

	private float CalculateHorizontalInput()
	{
		var forwardOnXZplane = CachedTransform.forward;
		var fw2d = new Vector3(forwardOnXZplane.x, forwardOnXZplane.z).normalized;

		var toPlayer = _player.position - CachedTransform.position;
		var toPlayer2d = new Vector2(toPlayer.x, toPlayer.z).normalized;

		var angle = Vector2.SignedAngle(fw2d, toPlayer2d);

		return -(angle / 180f);
	}
}
