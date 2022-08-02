using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[SerializeField]
	GameObject normalHud;
	[SerializeField]
	GameObject pauseHud;

	[SerializeField]
	public UITextPanel playerHealthPanel;
	[SerializeField]
	public UITextPanel targetPanel;
	[SerializeField]
	public UITextPanel ammoPanel;

	//[SerializeField]
	//PlayerController player;
	//[SerializeField]
	//HealthComponent playerHealth;
	//[SerializeField]
	//HealthComponent targetHealth;

	// Start is called before the first frame update
	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate UIManager!");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	void Start()
	{
		//player = PlayerController.instance;

		//if(player)
		//{
		//	playerHealth = player.GetHealthComponent();
		//}
	}

	public void SetTargetPanelText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			targetPanel.SetActive(true);
			targetPanel.SetText(text);
		}
		else
		{
			targetPanel.SetActive(false);
		}
	}

	public void SetAmmoText(float ammo, float totalAmmo = -1)
	{
		ammoPanel.SetText($"Ammo: {ammo}/{totalAmmo}");
	}


	// Update is called once per frame
	void Update()
	{
		//if (targetHealth)
		//{
		//	targetHealthPanel.SetActive(true);

		//	if (!targetHealth.IsDead())
		//	{
		//		targetHealthPanel.SetText($"Target Health: {targetHealth.GetHealth()} ({targetHealth.GetHealthPercent()}%)");
		//	}
		//	else
		//	{
		//		targetHealthPanel.SetText($"Target Health: DEAD");
		//	}
		//}
		//else
		//{
		//	targetHealthPanel.SetActive(false);
		//}

		//if (player)
		//{
		//	if (!playerHealth.IsDead())
		//	{
		//		playerHealthPanel.SetText($"Health: {playerHealth.GetHealth()} ({playerHealth.GetHealthPercent()}%)");
		//	}
		//	else
		//	{
		//		playerHealthPanel.SetText($"Health: DEAD");
		//	}

		//	float ammo = player.GetCurrentWeapon().GetCurrentAmmo();
		//	float totalAmmo = player.GetCurrentWeapon().GetTotalAmmo();

		//	ammoPanel.SetText($"Ammo: {ammo}/{totalAmmo}");
		//}
	}
}
