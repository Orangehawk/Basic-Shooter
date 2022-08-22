using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
	[SerializeField]
	float ammoAmount = 30;

	void OnTriggerEnter(Collider other)
	{
		//if (other.TryGetComponent<HealthComponent>(out HealthComponent hc))
		//{
		//	if (hc.GetHealthPercent() < 100)
		//	{
		//		hc.Heal(healAmount);
		//		Destroy(gameObject);
		//	}
		//}
	}
}
