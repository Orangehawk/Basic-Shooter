using UnityEngine;
using System.Collections;

namespace Pathfinding
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(AIPath))]
	[RequireComponent(typeof(Seeker))]
	public class AIPathfinder : VersionedMonoBehaviour
	{
		/// <summary>The object that the AI should move to</summary>
		public Transform target;
		protected IAstarAI ai;
		protected AIPath aiPath;

		protected virtual void OnEnable()
		{
			ai = GetComponent<IAstarAI>();
			aiPath = GetComponent<AIPath>();

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
			this.target = target;
		}

		protected void UpdateDestination()
		{
			if (target != null && ai != null)
			{
				ai.destination = target.position;
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
}