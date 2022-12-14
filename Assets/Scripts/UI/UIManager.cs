using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[SerializeField]
	GameObject playingHud;
	[SerializeField]
	GameObject pausedHud;
	[SerializeField]
	GameObject gameWonHud;
	[SerializeField]
	GameObject gameLostHud;

	[SerializeField]
	public UITextPanel targetPanel;
	[SerializeField]
	public UITextPanel ammoPanel;
	[SerializeField]
	UIFlash hitEffect;
	//[SerializeField]
	//float hitEffectIncrease = 0.05f;

	[SerializeField]
	UIFlash healEffect;
	//[SerializeField]
	//float healEffectFade = 0.3f;

	// Start is called before the first frame update
	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate UIManager!");
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	void Start()
	{

	}

	public void SetHudLayer(string layer)
	{
		playingHud.SetActive(false);
		pausedHud.SetActive(false);
		gameWonHud.SetActive(false);
		gameLostHud.SetActive(false);

		switch (layer)
		{
			case "Playing":
				playingHud.SetActive(true);
				break;
			case "Paused":
				pausedHud.SetActive(true);
				break;
			case "GameWon":
				gameWonHud.SetActive(true);
				break;
			case "GameLost":
				gameLostHud.SetActive(true);
				break;
			default:
				Debug.LogWarning($"Invalid hud layer \"{layer}\"");
				break;
		}
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

	public void HitEffect()
	{
		hitEffect.Flash();
	}

	public void HealEffect()
	{
		healEffect.Flash();
	}

	void Update()
	{

	}
}
