using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Pathfinding.AIPath))]
[RequireComponent(typeof(Pathfinding.Seeker))]
public class AIPathfinder : MonoBehaviour
{
	/// <summary>The object that the AI should move to (overwrites position when set)</summary>
	public Transform target;
	/// <summary>The position that the AI should move to (overwrites transform when set)</summary>
	public Vector3? targetPosition;
	protected Pathfinding.IAstarAI ai;
	protected Pathfinding.AIPath aiPath;

	protected virtual void OnEnable()
	{
		ai = GetComponent<Pathfinding.IAstarAI>();
		aiPath = GetComponent<Pathfinding.AIPath>();

		// Update the destination right before searching for a path as well.
		// This is enough in theory, but this script will also update the destination every
		// frame as the destination is used for debugging and may be used for other things by other
		// scripts as well. So it makes sense that it is up to date every frame.
		if (ai != null)
		{
			ai.onSearchPath += UpdateDestination;
		}
	}

	protected virtual void OnDisable()
	{
		if (ai != null)
		{
			ai.onSearchPath -= UpdateDestination;
		}
	}

	protected void SetTarget(Transform target)
	{
		this.targetPosition = null;
		this.target = target;
	}

	protected void SetTargetPosition(Vector3 targetPosition)
	{
		this.target = null;
		this.targetPosition = targetPosition;
	}

	protected void UpdateDestination()
	{
		if (target != null && ai != null)
		{
			ai.destination = target.position;
		}
		else if (targetPosition != null && ai != null)
		{
			ai.destination = (Vector3)targetPosition;
		}
	}

	protected Transform GetTarget()
	{
		return target;
	}

	protected virtual void Update()
	{
		//Updates the AI's destination every frame
		UpdateDestination();
	}
}
