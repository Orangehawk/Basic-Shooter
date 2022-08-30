using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
	[SerializeField]
	string prefix;
	[SerializeField]
	UITextPanel textPanel;
	[SerializeField]
	HealthComponent healthComponent;
	//[SerializeField]
	//bool usePercent = false;

	float lastHealth = 0;

	// Start is called before the first frame update
	void Start()
	{
		if (healthComponent)
		{
			lastHealth = healthComponent.GetHealth();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (healthComponent)
		{
			textPanel.SetText($"{prefix}: {Mathf.Round(healthComponent.GetHealthPercent())}%");
			if (healthComponent.IsDead())
			{
				textPanel.SetBackgroundColor(Color.red);
			}
			else
			{
				if (healthComponent.GetHealth() != lastHealth)
				{
					textPanel.SetBackgroundColor(Color.Lerp(Color.red, Color.white, healthComponent.GetHealth() / healthComponent.GetMaxHealth()));
					lastHealth = healthComponent.GetHealth();
				}
			}
		}
	}
}
