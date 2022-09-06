using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerController : MonoBehaviour, IDamageable
{
	public static PlayerController instance;

	[Header("References")]
	[SerializeField]
	Transform groundCheckPos;
	[SerializeField]
	Transform grenadePos;
	[SerializeField]
	Weapon weapon;
	[SerializeField]
	AudioSource audioSource;
	[SerializeField]
	GameObject grenade;
	[SerializeField]
	Footsteps footsteps;

	[Header("Speed/Sensitivity")]
	[SerializeField]
	Vector2 mouseSensitivity = Vector2.one;
	[SerializeField]
	float walkSpeed = 5;
	[SerializeField]
	float sprintSpeed = 8;
	[SerializeField]
	float jumpHeight = 3;
	[SerializeField]
	float grenadeThrowForce = 5f;

	[SerializeField]
	uint grenades = 3;
	
	CharacterController characterController;
	HealthComponent healthComponent;
	Vector2 mouseInput = Vector2.zero;
	Vector3 playerInput = Vector3.zero;
	Camera cam;
	GameManager gameManager;
	UIManager uiManager;
	GameObject target;

	bool isSprinting = false;
	bool isJumping = false;
	bool isAiming = false;
	bool isInteracting = false;
	Vector3 cameraRotation = Vector3.zero;
	Vector3 playerVelocity = Vector3.zero;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate PlayerController!");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		characterController = GetComponent<CharacterController>();
		healthComponent = GetComponent<HealthComponent>();
		cam = Camera.main;
		gameManager = GameManager.instance;
		uiManager = UIManager.instance;

		if(weapon)
		{
			weapon.SetHeldByPlayer();
		}

		cameraRotation = cam.transform.localRotation.eulerAngles;

		healthComponent.onDamage += OnHit;
		healthComponent.onHeal += OnHeal;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent(out ICollectable collectable))
		{
			collectable.Collect(gameObject);
		}
	}

	public void PlayOneShot(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}

	public void ForceRotate(float horizontal, float vertical)
	{
		transform.Rotate(0, horizontal, 0);

		cameraRotation.x += vertical;

		cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);

		cam.transform.localRotation = Quaternion.Euler(cameraRotation);
	}

	public HealthComponent GetHealthComponent()
	{
		return healthComponent;
	}

	public Weapon GetCurrentWeapon()
	{
		return weapon;
	}

	public void AddAmmo(float amount)
	{
		weapon.AddAmmo(amount);
	}

	void ThrowGrenade()
	{
		if (grenades > 0)
		{
			Rigidbody r = Instantiate(grenade, grenadePos.position, transform.rotation).GetComponent<Rigidbody>();
			r.AddForce(cam.transform.forward * grenadeThrowForce, ForceMode.VelocityChange);
			grenades--;
		}
	}

	void GetInput()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			gameManager.TogglePausePlay();
		}


		if (gameManager.GetState() == GameManager.State.Playing)
		{
			mouseInput.x = Input.GetAxisRaw("Mouse X");
			mouseInput.y = -Input.GetAxisRaw("Mouse Y");
			mouseInput *= mouseSensitivity;

			if (Input.GetKey(KeyCode.W))
			{
				playerInput.z = 1;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				playerInput.z = -1;
			}
			else
			{
				playerInput.z = 0;
			}

			if (Input.GetKey(KeyCode.D))
			{
				playerInput.x = 1;
			}
			else if (Input.GetKey(KeyCode.A))
			{
				playerInput.x = -1;
			}
			else
			{
				playerInput.x = 0;
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				isJumping = true;
			}
			else
			{
				isJumping = false;
			}

			if (Input.GetKey(KeyCode.LeftShift))
			{
				isSprinting = true;
			}
			else
			{
				isSprinting = false;
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				weapon.Reload();
			}

			if (Input.GetMouseButton(0))
			{
				weapon.Fire();
			}

			if (Input.GetMouseButton(1))
			{
				weapon.SetAiming(true);
			}
			else
			{
				weapon.SetAiming(false);
			}

			if(Input.GetKeyDown(KeyCode.E))
			{
				isInteracting = true;
			}
			else
			{
				isInteracting = false;
			}

			if (Input.GetKeyDown(KeyCode.G))
			{
				ThrowGrenade();
			}

			if (Input.GetKeyDown(KeyCode.Minus))
			{
				Minimap.instance.Zoom(false);
			}

			if (Input.GetKeyDown(KeyCode.Equals))
			{
				Minimap.instance.Zoom(true);
			}

			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				Minimap.instance.ResetZoom();
			}
		}
	}

	bool IsGrounded()
	{
		return Physics.CheckBox(transform.position + groundCheckPos.localPosition, groundCheckPos.localScale/2, Quaternion.identity, ~LayerMask.GetMask("Player", "Projectiles", "Ignore Raycast"), QueryTriggerInteraction.Ignore);
	}

	void HandleMovement()
	{
		Vector3 direction = ((transform.forward * playerInput.z) + (transform.right * playerInput.x)).normalized;

		if (!isSprinting)
		{
			characterController.Move(direction * walkSpeed * Time.deltaTime);
		}
		else
		{
			characterController.Move(direction * sprintSpeed * Time.deltaTime);
		}

		if (IsGrounded())
		{
			if (playerVelocity.y < 0)
			{
				playerVelocity.y = -2f;
			}

			if (isJumping)
			{
				playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * -Physics.gravity.magnitude);
			}
		}

		playerVelocity.y += -Physics.gravity.magnitude * Time.deltaTime;
		characterController.Move(playerVelocity * Time.deltaTime);

		footsteps.SetIsGrounded(IsGrounded());
	}

	void HandleRotation()
	{
		//Mouse input is already deltatime-d
		cameraRotation.x += mouseInput.y;

		transform.Rotate(0, mouseInput.x, 0);

		cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);

		cam.transform.localRotation = Quaternion.Euler(cameraRotation);
	}

	void HandleRaycastTarget()
	{
		target = null;
		int layerMask = ~LayerMask.GetMask("Player", "Projectiles");

		RaycastHit hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask, QueryTriggerInteraction.Ignore))
		{
			target = hit.collider.gameObject;

			if (hit.collider.gameObject.TryGetComponent(out IInteractable temp))
			{
				if(isInteracting)
				{
					temp.Interact();
				}
				//Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
			}
			else
			{
				//Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
			}
		}
		else
		{
			//Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * 100, Color.blue);
		}

	}

	void UpdateUI()
	{
		uiManager.SetAmmoText(weapon.GetCurrentAmmo(), weapon.GetTotalAmmo());

		if (target && target.TryGetComponent(out IInteractable interactable))
		{
			uiManager.SetTargetPanelText($"[{"E"}] {interactable.OnHover()}");
		}
		else if (target && target.TryGetComponent(out IDisplayable displayable))
		{
			uiManager.SetTargetPanelText(displayable.OnHover());
		}
		else
		{
			uiManager.SetTargetPanelText("");
		}
	}

	void OnHit()
	{
		uiManager.HitEffect();
	}

	void OnHeal()
	{
		uiManager.HealEffect();
	}

	// Update is called once per frame
	void Update()
	{
		if (!healthComponent.IsDead())
		{
			GetInput();

			if (gameManager.GetState() == GameManager.State.Playing)
			{
				HandleRotation();
				HandleMovement();
				HandleRaycastTarget();
				UpdateUI();
			}
		}
		else
		{
			gameManager.SetState(GameManager.State.GameLost);
		}
	}

	public void Damage(float amount)
	{
		healthComponent.Damage(amount);
	}

	public bool Heal(float amount)
	{
		return healthComponent.Heal(amount);
	}

	public void Kill()
	{
		healthComponent.Kill();
	}
}
