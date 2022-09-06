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
			if (collision.gameObject.TryGetComponent(out IDamageable damageable))
			{
				damageable.Damage(damage);
			}

			BulletHitManager.instance.CreateBulletHit(collision.GetContact(0).otherCollider.sharedMaterial, collision.GetContact(0).point, Quaternion.FromToRotation(Vector3.forward, collision.GetContact(0).normal));
			//GameObject o = Instantiate(hit.particle, collision.GetContact(0).point, Quaternion.FromToRotation(Vector3.forward, collision.GetContact(0).normal));
			//o.transform.rotation = Quaternion.FromToRotation(Vector3.forward, collision.GetContact(0).normal);

			//AudioSource.PlayClipAtPoint(hit.sound, collision.GetContact(0).point);

			Destroy(gameObject);
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
