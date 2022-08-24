using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable, IDisplayable
{
	[SerializeField]
	GameObject interactableObject;

	Animator animator;
	IControllableSystem controllableSystem;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();

		controllableSystem = interactableObject.GetComponent<IControllableSystem>();

		if(controllableSystem == null)
		{
			Debug.LogError($"Failed to find interaction on {interactableObject.name}, destroying this {name}");
			Destroy(this);
		}
	}

	public void Interact()
	{
		animator.Play("Press");
		controllableSystem.Action();
	}

	public string OnHover()
	{
		return controllableSystem.DisplayText();
	}


    // Update is called once per frame
    void Update()
    {
        
    }
}
