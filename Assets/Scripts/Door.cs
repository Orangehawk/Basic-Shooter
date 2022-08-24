using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IControllableSystem
{
	Animator animator;
	bool isOpen = false;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();

	}

	public void Action()
	{
		Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			if (isOpen)
			{
				animator.Play("Close");
			}
			else
			{
				animator.Play("Open");
			}

			isOpen = !isOpen;
		}
	}

	public string DisplayText()
	{
		return $"{(isOpen ? "Close" : "Open")} Door";
	}

	// Update is called once per frame
	void Update()
	{

	}
}
