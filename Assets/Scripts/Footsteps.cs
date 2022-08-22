using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	Vector3 lastPos = Vector3.zero;
	float footstepDistance = 0;

	// Start is called before the first frame update
	void Start()
    {
        
    }

	void HandleFootsteps()
	{
		if (playerInput.z != 0 || playerInput.x != 0)
		{
			footstepDistance += Vector3.Distance(transform.position, lastPos);
			lastPos = transform.position;
		}
		else
		{
			footstepDistance = 0;
		}

		if (footstepDistance >= 2)
		{
			footstepAudio.Play();
			footstepDistance = 0;
		}
	}

	// Update is called once per frame
	void Update()
    {
		HandleFootsteps();
    }
}
