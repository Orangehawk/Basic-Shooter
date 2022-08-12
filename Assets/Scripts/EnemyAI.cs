using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class EnemyAI : AIPathfinder
	{
		enum State
		{
			Idle,
			Aware,
			Patrolling,
			Hit,
			Searching,
			Fighting,
			Escaping
		}

		[Header("References")]
		[SerializeField]
		Weapon weapon;
		[SerializeField]
		Transform eyesPosition;

		[Header("Options")]
		[SerializeField]
		State defaultState = State.Idle;
		[SerializeField]
		float rotateSpeed = 1;
		[SerializeField]
		float defaultSlowdownDistance = 0.6f;
		[SerializeField]
		float defaultEndReachedDistance = 0.2f;
		[SerializeField]
		float chaseSlowdownDistance = 5;
		[SerializeField]
		float chaseEndReachedDistance = 3;

		[Header("Idle State")]
		[SerializeField]
		float idleRotateTime = 5;
		[SerializeField]
		float idleRotateChance = 40;
		[SerializeField]
		float idleMaxRotateAmount = 40;
		[SerializeField]
		float idleMinRotateAmount = 20;

		[Header("Aware State")]
		[SerializeField]
		float maxAwareTime = 20;
		[SerializeField]
		float awareRotateTime = 2.5f;
		[SerializeField]
		float awareRotateChance = 80;
		[SerializeField]
		float awareMaxRotateAmount = 40;
		[SerializeField]
		float awareMinRotateAmount = 20;

		[Header("Patrol State")]
		[SerializeField]
		Vector3 maxPatrolDistance = new Vector3(10, 0, 10);
		[SerializeField]
		Transform moveTarget;
		[SerializeField]
		float timeReachedEndOfPath = 0;
		[SerializeField]
		float patrolWaitAtEndOfPathTime = 5;
		[SerializeField]
		bool atEndOfPathLastFrame = false; //True if at the end of the path in the last frame

		[Header("Hit State")]

		[Header("Searching State")]
		[SerializeField]
		float maxSearchTime = 20;
		[SerializeField]
		Vector3 maxSearchDistance = new Vector3(10, 0, 10);
		[SerializeField]
		float searchWaitAtEndOfPathTime = 2;

		[Header("Fighting State")]
		[SerializeField]
		float angleToStartShooting = 5;

		[Header("Escaping State")]

		[Header("Debug")]
		[SerializeField]
		CharacterController controller;
		[SerializeField]
		Vector3 lastTargetLocation;
		[SerializeField]
		Vector3 targetRotation;
		[SerializeField]
		State currentState;

		[SerializeField]
		float rotateAmount = 0;
		[SerializeField]
		float lastStateChange = 0;
		[SerializeField]
		bool stateInitialised = false; //Generic initialised variable for states to manage their init
		[SerializeField]
		float generalStateTimer = 0; //Timer variable used however the active state wants
		[SerializeField]
		float rotationTimer = 0;

		void Start()
		{
			controller = GetComponent<CharacterController>();

			moveTarget = new GameObject("Move target").transform;

			SetState(defaultState);
		}

		void SetState(State state)
		{
			if (currentState != state)
			{
				Debug.Log($"{gameObject.name} entered state \"{state}\"");
				currentState = state;
				stateInitialised = false;
				lastStateChange = Time.time;
			}
		}

		State GetState()
		{
			return currentState;
		}

		void HandleState()
		{
			switch (GetState())
			{
				case State.Idle:
					IdleState();
					break;
				case State.Aware:
					AwareState();
					break;
				case State.Patrolling:
					PatrollingState();
					break;
				case State.Hit:
					HitState();
					break;
				case State.Searching:
					SearchingState();
					break;
				case State.Fighting:
					FightingState();
					break;
				case State.Escaping:
					EscapingState();
					break;
				default:
					break;
			}
		}

		bool WeaponReady()
		{
			if(weapon && weapon.GetTotalAmmo() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void SawTarget(Transform target)
		{
			SetTarget(target);
			if (WeaponReady())
			{
				SetState(State.Fighting);
				aiPath.enableRotation = false;
				aiPath.slowdownDistance = chaseSlowdownDistance;
				aiPath.endReachedDistance = chaseEndReachedDistance;
			}
			else
			{
				SetState(State.Escaping);
				aiPath.enableRotation = true;
				aiPath.slowdownDistance = defaultSlowdownDistance;
				aiPath.endReachedDistance = defaultEndReachedDistance;
			}
		}

		void LostTarget()
		{
			lastTargetLocation = GetTarget().position;
			SetTarget(null);
			if (GetState() == State.Fighting)
			{
				SetState(State.Searching);
			}
			aiPath.enableRotation = true;
			aiPath.slowdownDistance = defaultSlowdownDistance;
			aiPath.endReachedDistance = defaultEndReachedDistance;
		}

		float GetRandomClampedRange(float lowMin, float lowMax, float highMin, float highMax)
		{
			float amount;

			if(Random.Range(-1f, 1f) < 0)
			{
				amount = Random.Range(lowMin, lowMax);
			}
			else
			{
				amount = Random.Range(highMin, highMax);
			}

			return amount;
		}

		void IdleState()
		{
			if (Time.time >= generalStateTimer + idleRotateTime)
			{
				float randomNum = Random.Range(0, 100);

				if (randomNum < idleRotateChance)
				{
					rotateAmount = GetRandomClampedRange(-idleMaxRotateAmount, -idleMinRotateAmount, idleMinRotateAmount, idleMaxRotateAmount);
					rotationTimer = 0;
					targetRotation = transform.rotation.eulerAngles + new Vector3(0, rotateAmount, 0);
				}

				generalStateTimer = Time.time;
			}

			if (transform.rotation.eulerAngles != targetRotation)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), rotationTimer * Time.deltaTime);
				rotationTimer += rotateSpeed * Time.deltaTime;
			}
		}

		void AwareState()
		{
			if (Time.time >= lastStateChange + maxAwareTime)
			{
				SetState(State.Idle);
			}

			if (Time.time >= generalStateTimer + awareRotateTime)
			{
				float randomNum = Random.Range(0, 100);

				if (randomNum < awareRotateChance)
				{
					//rotateAmount = Random.Range(-awareMaxRotateAmount, awareMaxRotateAmount);
					rotateAmount = GetRandomClampedRange(-awareMaxRotateAmount, -awareMinRotateAmount, awareMinRotateAmount, awareMaxRotateAmount);
					rotationTimer = 0;
					targetRotation = transform.rotation.eulerAngles + new Vector3(0, rotateAmount, 0);
				}

				generalStateTimer = Time.time;
			}

			if (transform.rotation.eulerAngles != targetRotation)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), rotationTimer * Time.deltaTime);
				rotationTimer += rotateSpeed * Time.deltaTime;
			}
		}

		void GetNewMoveTarget(Vector3 centre, Vector3 maxDistance)
		{
			bool successfullyFound = false;

			int layermask = ~LayerMask.GetMask("Ground");
			Vector3 newPos = Vector3.zero;
			while (!successfullyFound)
			{
				newPos = centre + new Vector3(Random.Range(-maxDistance.x, maxDistance.x), Random.Range(-maxDistance.y, maxDistance.y), Random.Range(-maxDistance.z, maxDistance.z));
				if (!Physics.CheckBox(newPos, Vector3.one / 2f, Quaternion.identity, layermask))
				{
					successfullyFound = true;
				}
			}

			moveTarget.position = newPos;
		}

		void PatrollingState()
		{
			if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
			{
				if (atEndOfPathLastFrame)
				{
					generalStateTimer += Time.deltaTime; //Time since reached end of path
				}
				else
				{
					timeReachedEndOfPath = Time.time;
					atEndOfPathLastFrame = true;
				}
			}

			if (!stateInitialised || (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && Time.time >= timeReachedEndOfPath + patrolWaitAtEndOfPathTime))
			{
				GetNewMoveTarget(transform.position, maxPatrolDistance);
				SetTarget(moveTarget);
				UpdateDestination();
				ai.SearchPath();
				atEndOfPathLastFrame = false;
				stateInitialised = true;
			}
		}

		//TODO: Replace with custom hit state values
		void HitState()
		{
			Vector3 targetDirection = lastTargetLocation - transform.position;
			targetDirection.y = 0;

			float singleStep = rotateSpeed * 2 * Time.deltaTime;

			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

			Debug.DrawRay(transform.position, newDirection, Color.magenta);

			transform.rotation = Quaternion.LookRotation(newDirection);

			if (Vector3.Angle(targetDirection, transform.forward) <= angleToStartShooting && GetTarget() == null)
			{
				SetState(State.Aware);
			}
		}

		void SearchingState()
		{
			if (Time.time >= lastStateChange + maxSearchTime)
			{
				SetState(State.Aware);
			}

			if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
			{
				if (atEndOfPathLastFrame)
				{
					generalStateTimer += Time.deltaTime; //Time since reached end of path
				}
				else
				{
					timeReachedEndOfPath = Time.time;
					atEndOfPathLastFrame = true;
				}
			}

			if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && Time.time >= timeReachedEndOfPath + searchWaitAtEndOfPathTime)
			{
				GetNewMoveTarget(lastTargetLocation, maxSearchDistance);
				SetTarget(moveTarget);
				UpdateDestination();
				ai.SearchPath();
				atEndOfPathLastFrame = false;
			}
		}

		void FightingState()
		{
			if (true)//ai.reachedEndOfPath)
			{
				Vector3 targetDirection = target.position - transform.position;
				targetDirection.y = 0;

				float singleStep = rotateSpeed * Time.deltaTime;

				Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

				Debug.DrawRay(transform.position, newDirection, Color.red);

				transform.rotation = Quaternion.LookRotation(newDirection);

				if(Vector3.Angle(targetDirection, transform.forward) <= angleToStartShooting)
				{
					if (weapon.GetTotalAmmo() > 0)
					{
						if (weapon.GetCurrentAmmo() > 0)
						{
							weapon.Fire();
						}
						else
						{
							weapon.Reload();
						}
					}
					else
					{
						aiPath.enableRotation = true;
						SetState(State.Escaping);
					}
				}
			}
		}

		void EscapingState()
		{
			//Run from last known player position
		}

		void DeadState()
		{

		}

		void OnCollisionEnter(Collision collision)
		{
			//Handle state after being hit by a bullet
			if(collision.collider.CompareTag("Projectile"))
			{
				if (GetState() != State.Fighting && GetState() != State.Escaping)
				{
					lastTargetLocation = collision.transform.position;

					if(WeaponReady())
					{
						SetState(State.Hit);
					}
				}
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				RaycastHit hit;
				if (Physics.Raycast(eyesPosition.position, other.transform.position - transform.position, out hit))
				{
					if (hit.collider.CompareTag("Player"))
					{
						Debug.Log($"{name} saw Player");
						SawTarget(other.transform);
						return;
					}
				}

				//If we didn't see the target directly
				if (GetTarget() != null)
				{
					Debug.Log($"{name} saw Player");
					LostTarget();
				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				//SawTarget(other.transform);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player") && GetTarget() != null)
			{
				Debug.Log($"{name} lost Player");
				LostTarget();
			}
		}

		protected override void Update()
		{
			base.Update();
			HandleState();
		}
	}
}