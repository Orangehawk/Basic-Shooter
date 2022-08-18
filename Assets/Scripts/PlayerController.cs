using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	[Header("References")]
	[SerializeField]
	Transform groundCheckPos;
	[SerializeField]
	Weapon weapon;
	[SerializeField]
	Vector3 weaponHipPosition;
	[SerializeField]
	Vector3 weaponAimPosition;

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
	float aimSpeed = 0.2f;

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
	Vector3 weaponVelocity = Vector3.zero;
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

		cameraRotation = cam.transform.localRotation.eulerAngles;
	}

	public HealthComponent GetHealthComponent()
	{
		return healthComponent;
	}

	public Weapon GetCurrentWeapon()
	{
		return weapon;
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
				isAiming = true;
			}
			else
			{
				isAiming = false;
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
	}

	void HandleRotation()
	{
		//Mouse input is already deltatime-d
		cameraRotation.x += mouseInput.y;

		transform.Rotate(0, mouseInput.x, 0);

		cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);

		cam.transform.localRotation = Quaternion.Euler(cameraRotation);
	}

	void HandleAiming()
	{
		if (isAiming)
		{
			weapon.transform.localPosition = Vector3.SmoothDamp(weapon.transform.localPosition, weaponAimPosition, ref weaponVelocity, aimSpeed);
		}
		else
		{
			weapon.transform.localPosition = Vector3.SmoothDamp(weapon.transform.localPosition, weaponHipPosition, ref weaponVelocity, aimSpeed);
		}
	}

	void HandleRaycastTarget()
	{
		target = null;
		int layerMask = ~LayerMask.GetMask("Player", "Projectiles");

		RaycastHit hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask, QueryTriggerInteraction.Ignore))
		{
			target = hit.collider.gameObject;

			HealthComponent temp;
			if (hit.collider.gameObject.TryGetComponent(out temp))
			{
				Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
			}
			else
			{
				Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
			}
		}
		else
		{
			Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * 100, Color.blue);
		}

	}

	void UpdateUI()
	{
		uiManager.SetAmmoText(weapon.GetCurrentAmmo(), weapon.GetTotalAmmo());

		HealthComponent h;
		if (target && target.TryGetComponent(out h))
		{
			uiManager.SetTargetPanelText($"Target Health: {h.GetHealthPercent()}");
		}
		else
		{
			uiManager.SetTargetPanelText("");
		}
	}

	// Update is called once per frame
	void Update()
	{
		GetInput();

		if (gameManager.GetState() == GameManager.State.Playing)
		{
			HandleRotation();
			HandleMovement();
			HandleAiming();
			HandleRaycastTarget();
			UpdateUI();
		}
	}
}
