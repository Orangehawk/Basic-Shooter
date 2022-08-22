using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
	
	[SerializeField]
	float healAmount = 25;

	//void OnTriggerEnter(Collider other)
	//{
	//	if (other.TryGetComponent<HealthComponent>(out HealthComponent hc))
	//	{
	//		if (hc.GetHealthPercent() < 100)
	//		{
	//			hc.Heal(healAmount);
	//			Destroy(gameObject);
	//		}
	//	}
	//}
}
