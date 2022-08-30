using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAwaken : MonoBehaviour
{
    [SerializeField]
    Animator cannonAnimator;
    [SerializeField]
    Animator coverAnimator;
    [SerializeField]
    ParticleSystem dustParticles;
    [SerializeField]
    ParticleSystem fireParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ProcessInput()
	{
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            coverAnimator.Play("Open");
            dustParticles.Play();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            coverAnimator.Play("Close");
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
            cannonAnimator.Play("Extend");
            //dustParticles.Play();
		}
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            cannonAnimator.Play("Retract");
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            cannonAnimator.Play("Fire");
            fireParticles.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }
}
