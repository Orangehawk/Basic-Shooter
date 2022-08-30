using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
	[SerializeField]
	GameObject particles;
	[SerializeField]
	float explosionRadius = 20;
	[SerializeField]
	float damage = 40;
	[SerializeField]
	AnimationCurve damageCurve;
	[SerializeField]
	float initialVelocity = 3;
	[SerializeField]
	bool explodeOnHit = false;
	[SerializeField]
	float timeToExplode = 3;

	Rigidbody rb;
	float expireTime;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		expireTime = Time.time + timeToExplode;

		rb.AddRelativeForce(new Vector3(0, 0, initialVelocity), ForceMode.Impulse);
	}

	// Update is called once per frame
	void Update()
	{
		if(Time.time >= expireTime)
		{
			Explode();
		}
	}

	void Explode()
	{
		Instantiate(particles, transform.position, particles.transform.rotation);

		Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (Collider hit in hits)
		{
			Debug.Log(hit.GetType());
			if (hit.GetType() != typeof(CharacterController) && hit.gameObject.TryGetComponent(out HealthComponent hc))
			{
				if(Physics.Linecast(transform.position, hit.transform.position, out RaycastHit info))
				{
					if(info.collider == hit)
					{
						hc.Damage(damage * damageCurve.Evaluate(1f - (info.distance / explosionRadius)));
					}
				}
			}
		}

		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(explodeOnHit)
		{
			Explode();
		}
	}
}
