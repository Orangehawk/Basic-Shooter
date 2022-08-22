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
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
