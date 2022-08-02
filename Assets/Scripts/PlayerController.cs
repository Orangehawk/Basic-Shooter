using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	[SerializeField]
	Weapon weapon;
	[SerializeField]
	Vector2 mouseSensitivity = Vector2.one;
	[SerializeField]
	float moveSpeed = 2;
	[SerializeField]
	float walkSpeed = 5;
	[SerializeField]
	float sprintSpeed = 8;

	Rigidbody rb;
	HealthComponent healthComponent;
	Vector2 mouseInput = Vector2.zero;
	Vector3 playerInput = Vector3.zero;
	Camera cam;
	UIManager uiManager;
	GameObject target;

	bool isSprinting = false;

	void Awake()
	{
		if(instance != null && instance != this)
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
		rb = GetComponent<Rigidbody>();
		healthComponent = GetComponent<HealthComponent>();
		cam = Camera.main;
		uiManager = UIManager.instance;

		Cursor.lockState = CursorLockMode.Locked;
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
		mouseInput.x = Input.GetAxis("Mouse X");
		mouseInput.y = Input.GetAxis("Mouse Y");
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
	}

	void HandleMovement()
	{
		if ((!isSprinting && rb.velocity.magnitude < walkSpeed) || (isSprinting && rb.velocity.magnitude < sprintSpeed))
		{
			rb.AddRelativeForce(playerInput);
		}
	}

	void HandleRotation()
	{
		transform.Rotate(0, mouseInput.x, 0);
		cam.transform.Rotate(-mouseInput.y, 0, 0);
	}

	void HandleRaycastTarget()
	{
		int layerMask = 1 << 8;
		layerMask = ~layerMask;
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask))
		{
			target = hit.collider.gameObject;

			HealthComponent temp;
			if(hit.collider.gameObject.TryGetComponent(out temp))
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
		if(target.TryGetComponent(out h))
		{
			uiManager.SetTargetPanelText($"Target Health: {h.GetHealthPercent()}");
		}
	}

	// Update is called once per frame
	void Update()
	{
		GetInput();
		HandleRotation();
		HandleMovement();
		HandleRaycastTarget();
		UpdateUI();
	}
}
