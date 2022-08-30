using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	[SerializeField]
	AudioSource footstepAudio;
	[SerializeField]
	float distanceBetweenSteps = 2;
	[SerializeField]
	AnimationCurve pitchRange = AnimationCurve.Linear(0, 0.7f, 1, 1);

	Vector3 lastPos = Vector3.zero;
	float footstepDistance = 0;
	bool isGrounded = false;

	void Awake()
	{
		lastPos = transform.position;
	}

	// Start is called before the first frame update
	void Start()
    {

	}

	public void SetIsGrounded(bool grounded)
	{
		this.isGrounded = grounded;
	}

	void HandleFootsteps()
	{
		if (isGrounded)
		{
			footstepDistance += Vector3.Distance(transform.position, lastPos);
			lastPos = transform.position;

			if (footstepDistance >= distanceBetweenSteps)
			{
				footstepAudio.pitch = pitchRange.Evaluate(Random.Range(0, 1f));
				footstepAudio.Play();
				footstepDistance = 0;
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
		HandleFootsteps();
    }
}
