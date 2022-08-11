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
	[SerializeField]
	bool enableTrail = false;

	Rigidbody rb;
	TrailMaker trailMaker;

	float expireTime;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddRelativeForce(0, 0, InitialVelocity, ForceMode.VelocityChange);

		expireTime = Time.time + lifeTime;

		if(enableTrail)
		{
			GameObject go = new GameObject("Trail");
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			trailMaker = go.AddComponent<TrailMaker>();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.collider.CompareTag("Projectile"))
		{
			Debug.Log($"Hit {collision.gameObject.name}");

			HealthComponent hc;
			if (collision.gameObject.TryGetComponent(out hc))
			{
				hc.Damage(damage);
			}

			Destroy(gameObject);
		}
		else
		{
			Debug.Log($"Hit {collision.gameObject.name} - no HC");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > expireTime)
		{
			Debug.Log($"Bullet expired");
			Destroy(gameObject);
		}
	}

	void OnDestroy()
	{
		trailMaker.gameObject.transform.parent = null;
	}
}
