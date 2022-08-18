using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public static int totalKills = 0;
	static int totalSpawns = 0;

	[SerializeField]
	GameObject prefab;
	[SerializeField]
	Transform parent;
	[SerializeField]
	EnemyAI.State defaultState = EnemyAI.State.Idle;
	[Tooltip("The interval between spawns, in seconds")]
	[SerializeField]
	float interval = 10;
	[Tooltip("The maximum amount of enemies _this_ spawner will spawn")]
	[SerializeField]
	int maxIndividualSpawns = 10;
	[Tooltip("The maximum amount of enemies _all_ spawners will spawn")]
	[SerializeField]
	int maxTotalSpawns = 10;

	int individualSpawns = 0;
	float lastSpawn = 0;

	// Start is called before the first frame update
	void Start()
	{
		SpawnEnemy();
	}

	void SpawnEnemy()
	{
		GameObject spawned;
		if (parent != null)
		{
			spawned = Instantiate(prefab, transform.position, transform.rotation, parent);
		}
		else
		{
			spawned = Instantiate(prefab, transform.position, transform.rotation);
		}

		EnemyAI ai;
		if (spawned.TryGetComponent<EnemyAI>(out ai))
		{
			ai.defaultState = defaultState;
		}

		lastSpawn = Time.time;
		individualSpawns++;
		totalSpawns++;
	}

	// Update is called once per frame
	void Update()
	{
		if (individualSpawns < maxIndividualSpawns && totalSpawns < maxTotalSpawns && Time.time > lastSpawn + interval)
		{
			SpawnEnemy();
		}

		if(totalKills == totalSpawns)
		{
			GameManager.instance.SetState(GameManager.State.GameWon);
		}
	}
}
