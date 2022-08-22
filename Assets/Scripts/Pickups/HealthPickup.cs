using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, ICollectable
{
	[SerializeField]
	float healAmount = 25;

	public void Collect(GameObject collector)
	{
		if (collector.TryGetComponent(out PlayerController player))
		{
			if (player.Heal(healAmount))
			{
				Destroy(gameObject);
			}
		}
	}
}
