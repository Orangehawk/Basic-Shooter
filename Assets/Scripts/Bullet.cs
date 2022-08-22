using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField]
	public float InitialVelocity = 1;
	[SerializeField]
	public float lifeTime = 5f;
	[SerializeField]
	public float damage = 1;
	[SerializeField]
	bool enableTrail = false;

	Rigidbody rb;
	[SerializeField]
	GameObject trailMaker;
	[SerializeField]
	GameObject particlesPrefab;

	float expireTime;
	bool isQuitting = false;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddRelativeForce(0, 0, InitialVelocity, ForceMode.VelocityChange);

		expireTime = Time.time + lifeTime;

		if(enableTrail && !trailMaker)
		{
			trailMaker = new GameObject("Trail");
			trailMaker.transform.parent = transform;
			trailMaker.transform.localPosition = Vector3.zero;
			trailMaker.AddComponent<TrailMaker>();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.collider.CompareTag("Projectile"))
		{
			//Debug.Log($"Hit {collision.gameObject.name}");

			HealthComponent hc;
			if (collision.gameObject.TryGetComponent(out hc))
			{
				hc.Damage(damage);
			}

			if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
			{
				GameObject o = Instantiate(particlesPrefab, collision.GetContact(0).point, Quaternion.identity);//.transform.localScale = transform.localScale;
				o.transform.forward = collision.GetContact(0).normal;
			}
			Destroy(gameObject);
		}
		else
		{
			//Debug.Log($"Hit {collision.gameObject.name} - no HC");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > expireTime)
		{
			//Debug.Log($"Bullet expired");
			Destroy(gameObject);
		}
	}

	void OnApplicationQuit()
	{
		isQuitting = true;
	}

	void OnDestroy()
	{
		if (!isQuitting)
		{
			//trailMaker.transform.parent = null;
		}
	}
}
