using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField]
	public float InitialVelocity = 1;
	[SerializeField]
	float lifeTime = 5f;
	[SerializeField]
	float damage = 1;

	Rigidbody rb;

	float expireTime;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddRelativeForce(0, 0, InitialVelocity, ForceMode.VelocityChange);

		expireTime = Time.time + lifeTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.collider.CompareTag("Projectile"))
		{
			HealthComponent hc;
			if (collision.gameObject.TryGetComponent<HealthComponent>(out hc))
			{
				hc.Damage(damage);
			}

			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > expireTime)
		{
			Destroy(gameObject);
		}
	}
}
