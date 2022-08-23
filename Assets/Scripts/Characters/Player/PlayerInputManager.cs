using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public Vector3 movementInput;
    public Vector3 rotationInput;

    private void Awake()
	{
		if(instance != null)
		{
            Debug.LogWarning("Duplicate PlayerInput!");
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
        
    }

 //   void GetInput()
	//{
	//	if (Input.GetKeyDown(KeyCode.P))
	//	{
	//		gameManager.TogglePausePlay();
	//	}


	//	if (gameManager.GetState() == GameManager.State.Playing)
	//	{
	//		mouseInput.x = Input.GetAxisRaw("Mouse X");
	//		mouseInput.y = -Input.GetAxisRaw("Mouse Y");
	//		mouseInput *= mouseSensitivity;

	//		if (Input.GetKey(KeyCode.W))
	//		{
	//			playerInput.z = 1;
	//		}
	//		else if (Input.GetKey(KeyCode.S))
	//		{
	//			playerInput.z = -1;
	//		}
	//		else
	//		{
	//			playerInput.z = 0;
	//		}

	//		if (Input.GetKey(KeyCode.D))
	//		{
	//			playerInput.x = 1;
	//		}
	//		else if (Input.GetKey(KeyCode.A))
	//		{
	//			playerInput.x = -1;
	//		}
	//		else
	//		{
	//			playerInput.x = 0;
	//		}

	//		if (Input.GetKeyDown(KeyCode.Space))
	//		{
	//			isJumping = true;
	//		}
	//		else
	//		{
	//			isJumping = false;
	//		}

	//		if (Input.GetKey(KeyCode.LeftShift))
	//		{
	//			isSprinting = true;
	//		}
	//		else
	//		{
	//			isSprinting = false;
	//		}

	//		if (Input.GetKeyDown(KeyCode.R))
	//		{
	//			weapon.Reload();
	//		}

	//		if (Input.GetMouseButton(0))
	//		{
	//			weapon.Fire();
	//		}

	//		if (Input.GetMouseButton(1))
	//		{
	//			weapon.SetAiming(true);
	//		}
	//		else
	//		{
	//			weapon.SetAiming(false);
	//		}
	//	}
	//}

    // Update is called once per frame
    void Update()
    {
        
    }
}
