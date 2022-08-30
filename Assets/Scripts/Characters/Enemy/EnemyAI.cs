using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AIPathfinder, IDisplayable, IDamageable
{
	public enum State
	{
		Idle,
		Aware,
		Patrolling,
		Hit,
		Searching,
		Fighting,
		Escaping,
		Dead
	}

	[Header("References")]
	[SerializeField]
	Weapon weapon;
	[SerializeField]
	Transform eyesPosition;
	[SerializeField]
	CharacterController characterController;
	[SerializeField]
	HealthBar healthBar;
	[SerializeField]
	FaceCamera faceCamera;

	[Header("Options")]
	[SerializeField]
	float fieldOfVision = 80;
	[SerializeField]
	public State defaultState = State.Idle;
	[SerializeField]
	float rotateSpeed = 1;
	[SerializeField]
	float deathCleanupTime = 5;
	[SerializeField]
	float defaultSlowdownDistance = 0.6f;
	[SerializeField]
	float defaultEndReachedDistance = 0.2f;
	[SerializeField]
	float chaseSlowdownDistance = 15;
	[SerializeField]
	float chaseEndReachedDistance = 10;

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
	[SerializeField]
	float maxChaseTime = 5;

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
	[SerializeField]
	float fightingRotateSpeed = 3;


	//[Header("Escaping State")]

	[Header("Debug")]
	[SerializeField]
	bool drawFieldOfVision = false;
	[SerializeField]
	float fieldOfVisionDrawLength = 30;

	HealthComponent healthComponent;
	Vector3 lastTargetLocation;
	Vector3 targetRotation;
	State currentState;

	float lastRotateAmount = 0;
	float lastStateChange = 0;
	bool stateInitialised = false; //Generic initialised variable for states to manage their init
	float generalStateTimer = 0; //Timer variable used however the active state wants
	float rotationTimer = 0;

	void Start()
	{
		healthComponent = GetComponent<HealthComponent>();
		characterController = GetComponent<CharacterController>();
		SetState(defaultState);

		healthComponent.onDamage += OnHit;
	}

	public void SetCanvasAndCamera(Canvas canvas, Camera cam)
	{
		healthBar.canvas = canvas;
		faceCamera.cam = cam;
	}

	void SetState(State state)
	{
		if (currentState != state)
		{
			//Debug.Log($"{gameObject.name} entered state \"{state}\"");
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
			case State.Dead:
				DeadState();
				break;
			default:
				break;
		}
	}

	bool WeaponReady()
	{
		if (weapon && weapon.GetTotalAmmo() > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool InFieldOfVision(Transform target)
	{
		Vector3 direction = target.position - transform.position;
		float angle = Vector3.Angle(direction, transform.forward);

		if (Mathf.Abs(angle) <= fieldOfVision / 2f)
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
		if (GetState() == State.Fighting || GetState() == State.Hit)
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

		if (Random.Range(-1f, 1f) < 0)
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
		if (!stateInitialised)
		{
			generalStateTimer = Time.time;
			stateInitialised = true;
		}

		if (Time.time >= generalStateTimer + idleRotateTime)
		{
			float randomNum = Random.Range(0, 100);

			if (randomNum < idleRotateChance)
			{
				lastRotateAmount = GetRandomClampedRange(-idleMaxRotateAmount, -idleMinRotateAmount, idleMinRotateAmount, idleMaxRotateAmount);
				rotationTimer = 0;
				targetRotation = transform.rotation.eulerAngles + new Vector3(0, lastRotateAmount, 0);
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
		if (!stateInitialised)
		{
			generalStateTimer = Time.time;
			stateInitialised = true;
		}

		if (Time.time >= lastStateChange + maxAwareTime)
		{
			SetState(State.Idle);
		}

		if (Time.time >= generalStateTimer + awareRotateTime)
		{
			float randomNum = Random.Range(0, 100);

			if (randomNum < awareRotateChance)
			{
				lastRotateAmount = GetRandomClampedRange(-awareMaxRotateAmount, -awareMinRotateAmount, awareMinRotateAmount, awareMaxRotateAmount);
				rotationTimer = 0;
				targetRotation = transform.rotation.eulerAngles + new Vector3(0, lastRotateAmount, 0);
			}

			generalStateTimer = Time.time;
		}

		if (transform.rotation.eulerAngles != targetRotation)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), rotationTimer * Time.deltaTime);
			rotationTimer += rotateSpeed * Time.deltaTime;
		}
	}

	Vector3 GetNewMoveTarget(Vector3 centre, Vector3 maxDistance)
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

		return newPos;
	}

	void PatrollingState()
	{
		if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
		{
			if (!atEndOfPathLastFrame)
			{
				timeReachedEndOfPath = Time.time;
				atEndOfPathLastFrame = true;
			}
		}

		if (!stateInitialised || (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && Time.time >= timeReachedEndOfPath + patrolWaitAtEndOfPathTime))
		{
			SetTargetPosition(GetNewMoveTarget(transform.position, maxPatrolDistance));
			atEndOfPathLastFrame = false;
			stateInitialised = true;
		}
	}

	void HitState()
	{
		if (!stateInitialised)
		{
			aiPath.enableRotation = false;
			stateInitialised = true;
			rotationTimer = 0;
			generalStateTimer = Time.time;
		}

		if (Time.time > lastStateChange + maxChaseTime)
		{
			SetTarget(null);
			aiPath.enableRotation = true;
			SetState(State.Searching);
		}
		else
		{
			Vector3 targetDirection = target.position - transform.position;
			targetDirection.y = 0;

			float singleStep = fightingRotateSpeed * Time.deltaTime;

			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

			Debug.DrawRay(transform.position, newDirection, Color.red);

			transform.rotation = Quaternion.LookRotation(newDirection);

			if (Vector3.Angle(targetDirection, transform.forward) <= angleToStartShooting)
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

		if (!stateInitialised || (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && Time.time >= timeReachedEndOfPath + searchWaitAtEndOfPathTime))
		{
			SetTargetPosition(GetNewMoveTarget(lastTargetLocation, maxSearchDistance));
			UpdateDestination();
			ai.SearchPath();
			atEndOfPathLastFrame = false;
			stateInitialised = true;
		}
	}

	void FightingState()
	{
		Vector3 targetDirection = target.position - transform.position;
		targetDirection.y = 0;

		float singleStep = fightingRotateSpeed * Time.deltaTime;

		Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

		Debug.DrawRay(transform.position, newDirection, Color.red);

		transform.rotation = Quaternion.LookRotation(newDirection);

		if (Vector3.Angle(targetDirection, transform.forward) <= angleToStartShooting)
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

	void EscapingState()
	{
		//Run from last known player position
	}

	void DeadState()
	{
		if (!stateInitialised)
		{
			aiPath.enabled = false;
			characterController.enabled = false;
			transform.Rotate(0, 0, 90);
			transform.Translate(0, -0.5f, 0, Space.World);
			stateInitialised = true;
			EnemySpawner.totalKills++;
			Destroy(gameObject, deathCleanupTime);
		}
	}

	public void RecieveMessage(Transform target)
	{
		//Debug.Log($"{name} Recieved message");
		if (GetTarget() == null)
		{
			//Debug.Log($"Target null");
			lastTargetLocation = target.position;
			aiPath.enableRotation = true;
			aiPath.slowdownDistance = defaultSlowdownDistance;
			aiPath.endReachedDistance = defaultEndReachedDistance;

			if (WeaponReady())
			{
				//Debug.Log($"{name} Searching");
				SetState(State.Searching);
			}
			else
			{
				//Debug.Log($"{name} Escaping");
				SetState(State.Escaping);
			}
		}
		else
		{
			//Debug.Log($"{name} Target not null");
		}
	}

	void GroupMessage(Transform target)
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, 20);

		foreach(Collider hit in hits)
		{
			if (hit.GetType() != typeof(CharacterController) && hit.gameObject.TryGetComponent(out EnemyAI enemy) && enemy != this)
			{
				//Debug.Log($"{name} Sent message");
				enemy.RecieveMessage(target);
			}
		}
	}

	void OnHit()
	{
		if (GetState() != State.Fighting && GetState() != State.Hit && GetState() != State.Escaping && GetState() != State.Dead)
		{
			SetTarget(PlayerController.instance.transform);
			GroupMessage(GetTarget());

			if (WeaponReady())
			{
				SetState(State.Hit);
			}
			else
			{
				SetState(State.Escaping);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (GetState() != State.Dead)
		{
			if (other.CompareTag("Player"))
			{
				if (InFieldOfVision(other.transform) && Physics.Linecast(eyesPosition.position, other.transform.position, out RaycastHit hit, ~LayerMask.GetMask("Projectiles")))
				{
					if (hit.collider.CompareTag("Player"))
					{
						SawTarget(other.transform);
						Debug.DrawLine(eyesPosition.position, hit.point, Color.red);
						return;
					}
				}

				//If we didn't see the target directly
				if (GetTarget() != null)
				{
					LostTarget();
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (GetState() != State.Dead)
		{
			if (other.CompareTag("Player") && GetTarget() != null)
			{
				LostTarget();
			}
		}
	}

	void DebugDrawFieldOfVision()
	{
		Vector3 rightSide = Quaternion.AngleAxis(fieldOfVision / 2f, new Vector3(0, 1, 0)) * transform.forward * fieldOfVisionDrawLength;
		Vector3 leftSide = Quaternion.AngleAxis(-fieldOfVision / 2f, new Vector3(0, 1, 0)) * transform.forward * fieldOfVisionDrawLength;

		Debug.DrawRay(transform.position, rightSide, Color.green);
		Debug.DrawRay(transform.position, leftSide, Color.green);
	}

	protected override void Update()
	{
		if (!healthComponent.IsDead())
		{
			base.Update();


			if (drawFieldOfVision)
			{
				DebugDrawFieldOfVision();
			}
		}
		else
		{
			if (GetState() != State.Dead)
			{
				SetState(State.Dead);
			}
		}

		HandleState();
	}

	private void OnDestroy()
	{
		if (healthBar)
		{
			Destroy(healthBar.gameObject);
		}
	}

	public string OnHover()
	{
		return $"Health: {Mathf.Round(healthComponent.GetHealthPercent())}";
	}

	public void Damage(float amount)
	{
		healthComponent.Damage(amount);
	}

	public bool Heal(float amount)
	{
		return healthComponent.Heal(amount);
	}

	public void Kill()
	{
		healthComponent.Kill();
	}
}
