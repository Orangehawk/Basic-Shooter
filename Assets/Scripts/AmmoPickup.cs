using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, ICollectable
{
	[SerializeField]
	float ammoAmount = 30;

	public void Collect(GameObject collector)
	{
		if (collector.TryGetComponent(out PlayerController player))
		{
			player.AddAmmo(ammoAmount);
			Destroy(gameObject);
		}
	}
}
