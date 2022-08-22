using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public enum PickupType
	{
		Health,
		Ammo
	}

	[SerializeField]
	protected PickupType pickupType;

	[SerializeField]
	protected float amount = 30;

	public void Take()
	{
		Destroy(gameObject);
	}
}
