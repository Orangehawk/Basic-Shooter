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
	AudioSource audioSource;
	[SerializeField]
	AnimationCurve firePitchRange = AnimationCurve.Linear(0, 0.9f, 1, 1);

	[SerializeField]
	AudioClip fireAudio;
	[SerializeField]
	AudioClip reloadAudio;

	[SerializeField]
	Vector3 weaponHipPosition;
	[SerializeField]
	Vector3 weaponAimPosition;
	[SerializeField]
	Vector3 reloadAngle = new Vector3(0, 0, -25);

	[SerializeField]
	float aimSpeed = 0.2f;
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
	float hipHorizontalRecoil = 0;
	[SerializeField]
	float hipVerticalRecoil = 0;
	[SerializeField]
	float aimHorizontalRecoil = 0;
	[SerializeField]
	float aimVerticalRecoil = 0;

	bool isReloading = false;
	float lastShot = 0;
	float timeBetweenShots = 0;
	bool isAiming = false;
	Vector3 aimVelocity = Vector3.zero;
	//Vector3 reloadAngleVelocity = Vector3.zero;
	float reloadAngleVelocity = 0;

	bool heldByPlayer = false;
	PlayerController player;

	// Start is called before the first frame update
	protected virtual void Start()
	{
		timeBetweenShots = 1f / fireRate;
	}

	public void SetHeldByPlayer()
	{
		heldByPlayer = true;
		player = PlayerController.instance;
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

	void HandleAiming()
	{
		if (isAiming)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, weaponAimPosition, ref aimVelocity, aimSpeed);
		}
		else
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, weaponHipPosition, ref aimVelocity, aimSpeed);
		}
	}

	void HandleReloadAngle()
	{
		if (isReloading)
		{
			transform.localRotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.z, reloadAngle.z, ref reloadAngleVelocity, aimSpeed));
		}
		else
		{
			transform.localRotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.z, 0, ref reloadAngleVelocity, aimSpeed));
		}
	}

	public void SetAiming(bool aiming)
	{
		if (!isReloading)
		{
			isAiming = aiming;
		}
		else
		{
			isAiming = false;
		}
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

			audioSource.pitch = firePitchRange.Evaluate(Random.Range(0, 1f));
			audioSource.PlayOneShot(fireAudio);

			if(player)
			{
				if(isAiming)
				{
					player.ForceRotate(aimHorizontalRecoil, aimVerticalRecoil);
				}
				else
				{
					player.ForceRotate(hipHorizontalRecoil, hipVerticalRecoil);
				}
			}
		}
	}

	public void Reload()
	{
		if ((!isReloading && currentAmmo < magazineSize) || infiniteAmmo)
		{
			audioSource.pitch = 1;
			audioSource.PlayOneShot(reloadAudio);
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
		HandleAiming();
		HandleReloadAngle();
	}
}
