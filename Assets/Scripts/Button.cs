using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable, IDisplayable
{
	[SerializeField]
	GameObject interactableObject;

	Animator animator;
	IInteractable interaction;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();

		interaction = interactableObject.GetComponent<IInteractable>();

		if(interaction == null)
		{
			Debug.LogError($"Failed to find interaction on {interactableObject.name}, destroying this {name}");
			Destroy(this);
		}
	}

	public void Interact()
	{
		animator.Play("Press");
		interaction.Interact();
	}

	public string OnHover()
	{
		return interaction.OnHover();
	}


    // Update is called once per frame
    void Update()
    {
        
    }
}
