using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
	[SerializeField]
	int fps = 144;

	Rigidbody rb;
	HealthComponent healthComponent;
	Vector2 mouseInput = Vector2.zero;
	Vector3 playerInput = Vector3.zero;
	Camera cam;
	UIManager uiManager;
	GameObject target;

	[SerializeField]
	bool isGrounded;
	bool isSprinting = false;
	bool isJumping = false;
	[SerializeField]
	bool isAiming = false;
	float aimTimer = 0;
	Vector3 weaponVelocity = Vector3.zero;
	Vector3 cameraRotation = Vector3.zero;

	private void OnValidate()
	{
		Application.targetFrameRate = fps;
	}

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

		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = fps;
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		healthComponent = GetComponent<HealthComponent>();
		cam = Camera.main;
		uiManager = UIManager.instance;

		cameraRotation = cam.transform.localRotation.eulerAngles;

		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnTriggerEnter(Collider other)
	{
		isGrounded = true;

	}

	private void OnTriggerExit(Collider other)
	{
		isGrounded = false;
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
		mouseInput.x = Input.GetAxisRaw("Mouse X");
		mouseInput.y = -Input.GetAxisRaw("Mouse Y");
		mouseInput *= mouseSensitivity;


		if (Input.GetKey(KeyCode.W))
		{
			playerInput.z = moveSpeed;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			playerInput.z = -moveSpeed;
		}
		else
		{
			playerInput.z = 0;
		}

		if (Input.GetKey(KeyCode.D))
		{
			playerInput.x = moveSpeed;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			playerInput.x = -moveSpeed;
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

	void HandleMovement()
	{
		if (isGrounded)
		{
			if (isJumping)
			{
				rb.AddRelativeForce(0, jumpSpeed, 0, ForceMode.Impulse);
			}

			if ((!isSprinting && rb.velocity.magnitude < walkSpeed) || (isSprinting && rb.velocity.magnitude < sprintSpeed))
			{
				rb.AddRelativeForce(playerInput);
			}
		}
		else
		{
			if (allowAirMovement && rb.velocity.magnitude < walkSpeed)
			{
				rb.AddRelativeForce(playerInput * airSpeedMult);
			}
		}
	}

	void HandleRotation()
	{
		//cameraRotation.x += mouseInput.x;
		cameraRotation.x += mouseInput.y;

		transform.Rotate(0, mouseInput.x, 0);

		cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);
		//cam.transform.Rotate(-mouseInput.y, 0, 0);

		Debug.Log(Quaternion.Euler(cameraRotation).eulerAngles);

		cam.transform.localRotation = Quaternion.Euler(cameraRotation);
	}

	void HandleAiming()
	{
		if(isAiming)
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
		if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask))
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
		HandleRotation();
		HandleMovement();
	}

	void LateUpdate()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		GetInput();
		HandleAiming();
		HandleRaycastTarget();
		UpdateUI();
	}
}
