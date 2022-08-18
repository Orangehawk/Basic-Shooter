using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	[SerializeField]
	float healAmount = 25;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			HealthComponent hc = other.GetComponent<HealthComponent>();
			if(hc.GetHealthPercent() < 100)
			{
				hc.Heal(healAmount);
				Destroy(gameObject);
			}
		}
	}
}
