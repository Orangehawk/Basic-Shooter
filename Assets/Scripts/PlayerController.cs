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
	Weapon weapon;
	[SerializeField]
	Vector3 weaponHipPosition;
	[SerializeField]
	Vector3 weaponAimPosition;

	[Header("Speed/Sensitivity")]
	[SerializeField]
	Vector2 mouseSensitivity = Vector2.one;
	[SerializeField]
	float moveSpeed = 2;
	[SerializeField]
	float airSpeedMult = 0.25f;
	[SerializeField]
	float walkSpeed = 5;
	[SerializeField]
	float sprintSpeed = 8;
	[SerializeField]
	float jumpSpeed = 3;
	[SerializeField]
	float aimSpeed = 0.2f;

	[Header("Options")]
	[SerializeField]
	bool allowAirMovement = false;

	[Header("Debug")]
	CharacterController characterController;
	HealthComponent healthComponent;
	Vector2 mouseInput = Vector2.zero;
	Vector3 playerInput = Vector3.zero;
	Camera cam;
	GameManager gameManager;
	UIManager uiManager;
	GameObject target;

	[SerializeField]
	bool useFixedUpdateCamera = false;
	[SerializeField]
	bool useFixedUpdateMovement = false;
	[SerializeField]
	bool isGrounded;
	bool isSprinting = false;
	bool isJumping = false;
	[SerializeField]
	bool isAiming = false;
	float aimTimer = 0;
	Vector3 weaponVelocity = Vector3.zero;
	Vector3 cameraRotation = Vector3.zero;
	[SerializeField]
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

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ground"))
		{
			isGrounded = true;
		}
		else
		{
			//Debug.Log(other.tag);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Ground"))
		{
			isGrounded = false;
		}
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
			if (useFixedUpdateCamera)
			{
				mouseInput.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity.x;
				mouseInput.y += -Input.GetAxisRaw("Mouse Y") * mouseSensitivity.y;
			}
			else
			{
				mouseInput.x = Input.GetAxisRaw("Mouse X");
				mouseInput.y = -Input.GetAxisRaw("Mouse Y");
				mouseInput *= mouseSensitivity;
			}


			if (Input.GetKeyDown(KeyCode.Q))
			{
				useFixedUpdateCamera = !useFixedUpdateCamera;
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				useFixedUpdateMovement = !useFixedUpdateMovement;
			}

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

			//Stops two inputs from doubling accelleration
			//if(playerInput.x != 0 && playerInput.z != 0)
			//{
			//	playerInput *= 0.5f;
			//}

			if (Input.GetKey(KeyCode.Space))
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
		//var a = Physics.CheckBox(transform.position + new Vector3(0, -1, 0), new Vector3(0.4f, 0.025f, 0.4f), Quaternion.identity, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

		//if (a)
		//{
		//	//Debug.Log("True");
		//	return true;
		//}

		////Debug.Log("False");
		//return false;

		return Physics.CheckBox(transform.position + new Vector3(0, -1, 0), new Vector3(0.4f, 0.025f, 0.4f), Quaternion.identity, ~LayerMask.GetMask("Player", "Ignore Raycast"), QueryTriggerInteraction.Ignore);
	}

	void HandleMovement()
	{
		Vector3 direction = ((transform.forward * playerInput.z) + (transform.right * playerInput.x)).normalized;
		//isGrounded = characterController.isGrounded;

		if (IsGrounded())
		{
			if (playerVelocity.y < 0)
			{
				playerVelocity.y = 0f;
			}

			//if ((!isSprinting && characterController.velocity.magnitude < walkSpeed) || (isSprinting && characterController.velocity.magnitude < sprintSpeed))
			if (!isSprinting)
			{
				characterController.Move(direction * moveSpeed * Time.deltaTime);
				//characterController.AddRelativeForce(playerInput);
			}
			else
			{
				characterController.Move(direction * sprintSpeed * Time.deltaTime);
			}

			if (isJumping)
			{
				playerVelocity.y += Mathf.Sqrt(jumpSpeed * -3.0f * -Physics.gravity.magnitude);
				//characterController.AddRelativeForce(0, jumpSpeed, 0, ForceMode.Impulse);
			}
		}
		else
		{
			//Debug.Log("Not grounded");
			if (allowAirMovement)
			{
				characterController.Move(direction * moveSpeed * airSpeedMult * Time.deltaTime);
				//characterController.AddRelativeForce(playerInput * airSpeedMult);
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

		if (useFixedUpdateCamera)
		{
			mouseInput = Vector2.zero;
		}
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
		int layerMask = 1 << 8;
		layerMask = ~layerMask;
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

	void FixedUpdate()
	{
		if (useFixedUpdateCamera)
		{
			HandleRotation();
		}

		if (useFixedUpdateMovement)
		{
			HandleMovement();
		}
	}

	void LateUpdate()
	{

	}

	// Update is called once per frame
	void Update()
	{
		GetInput();

		if (gameManager.GetState() == GameManager.State.Playing)
		{
			if (!useFixedUpdateCamera)
			{
				HandleRotation();
			}

			if (!useFixedUpdateMovement)
			{
				HandleMovement();
			}
			HandleAiming();
			HandleRaycastTarget();
			UpdateUI();
		}
	}
}
