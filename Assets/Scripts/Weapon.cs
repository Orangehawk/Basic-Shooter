using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField]
	GameObject bulletPrefab;
	[SerializeField]
	Transform bulletSpawnPoint;

	[SerializeField]
	int fireRate = 30; //Bullets per second
	[SerializeField]
	int damage = 10; //Bullet damage
	[SerializeField]
	float muzzleVelocity = 20;
	[SerializeField]
	float magazineSize = 30;
	[SerializeField]
	float currentAmmo = 30;
	[SerializeField]
	float reloadTime = 2;
	[SerializeField]
	bool discardMagazine = false; //Throws away all extra ammo in the magazine when reloading
	[SerializeField]
	float totalAmmo = 120;
	[SerializeField]
	bool infiniteAmmo = false;

	[SerializeField]
	bool isReloading = false;
	[SerializeField]
	float lastShot = 0;
	[SerializeField]
	float timeBetweenShots = 0;

	// Start is called before the first frame update
	protected virtual void Start()
	{
		timeBetweenShots = 1f / fireRate;
	}

	public void AddAmmo(float amount)
	{
		totalAmmo += amount;
	}

	public float GetCurrentAmmo()
	{
		return currentAmmo;
	}

	public float GetTotalAmmo()
	{
		return totalAmmo;
	}

	public void Fire()
	{
		if (!isReloading && currentAmmo > 0 && Time.time > lastShot + timeBetweenShots)
		{
			GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
			bullet.GetComponent<Bullet>().InitialVelocity = muzzleVelocity;
			bullet.GetComponent<Bullet>().damage = damage;
			currentAmmo--;
			lastShot = Time.time;
		}
	}

	public void Reload()
	{
		if ((!isReloading && currentAmmo < magazineSize) || infiniteAmmo)
		{
			StartCoroutine(ReloadCoroutine());
		}
	}

	IEnumerator ReloadCoroutine()
	{
		isReloading = true;

		if (!infiniteAmmo)
		{
			if (!discardMagazine)
			{
				totalAmmo += currentAmmo;
			}

			currentAmmo = 0;
		}

		yield return new WaitForSeconds(reloadTime);

		if (!infiniteAmmo)
		{
			if (totalAmmo >= magazineSize)
			{
				currentAmmo = magazineSize;
				totalAmmo -= magazineSize;
			}
			else
			{
				currentAmmo = totalAmmo;
				totalAmmo = 0;
			}
		}
		else
		{
			currentAmmo = magazineSize;
		}

		isReloading = false;
	}

	// Update is called once per frame
	protected virtual void Update()
	{

	}
}
