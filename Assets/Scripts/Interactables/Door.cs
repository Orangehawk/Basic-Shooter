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

	public void SetDoor(bool open)
	{
		if (isOpen != open)
		{
			if (open)
			{
				animator.Play("Open");
			}
			else
			{
				animator.Play("Close");
			}

			isOpen = open;
		}
	}

	public void Action()
	{
		Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			SetDoor(!isOpen);
		}
	}

	public string DisplayText()
	{
		return $"{(isOpen ? "Close" : "Open")} Door";
	}

	public string GetState()
	{
		return isOpen ? "Open" : "Closed";
	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			SetDoor(true);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
