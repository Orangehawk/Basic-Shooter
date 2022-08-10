using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[SerializeField]
	GameObject playingHud;
	[SerializeField]
	GameObject pausedHud;

	[SerializeField]
	public UITextPanel targetPanel;
	[SerializeField]
	public UITextPanel ammoPanel;

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
		switch(layer)
		{
			case "Playing":
				playingHud.SetActive(true);
				pausedHud.SetActive(false);
				break;
			case "Paused":
				playingHud.SetActive(false);
				pausedHud.SetActive(true);
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


	// Update is called once per frame
	void Update()
	{

	}
}
