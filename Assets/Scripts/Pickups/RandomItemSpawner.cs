using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject item;
	[SerializeField]
	Transform itemParent;
	[SerializeField]
	Vector3 maxPos = new Vector3(50, 0, 50);

	[SerializeField]
	float heightOverride = 0;
	[SerializeField]
	int totalToSpawn = 5;

	Vector3 halfExtents;

	float amountSpawned = 0;
	int emergencyBreak;
	int counter;

	void SpawnItems()
	{
		Vector3 centre = item.GetComponent<BoxCollider>().center;
		halfExtents = item.GetComponent<BoxCollider>().size / 2;

		counter = 0;
		while (amountSpawned < totalToSpawn && counter < emergencyBreak)
		{
			Vector3 newPos;

			if (heightOverride != 0)
			{
				newPos = new Vector3(Random.Range(-maxPos.x, maxPos.x), heightOverride, Random.Range(-maxPos.z, maxPos.z));
			}
			else
			{
				newPos = new Vector3(Random.Range(-maxPos.x, maxPos.x), Random.Range(-maxPos.y, maxPos.y), Random.Range(-maxPos.z, maxPos.z));
			}

			if (!Physics.CheckBox(newPos + centre, halfExtents))
			{
				if (itemParent)
				{
					Instantiate(item, newPos, Quaternion.identity, itemParent);
				}
				else
				{
					Instantiate(item, newPos, Quaternion.identity);
				}
				amountSpawned++;
			}

			counter++;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		emergencyBreak = totalToSpawn * 400;
		SpawnItems();
	}

	// Update is called once per frame
	void Update()
	{
	}
}
