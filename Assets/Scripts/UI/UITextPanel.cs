using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITextPanel : MonoBehaviour
{
    [SerializeField]
    Image background;
    [SerializeField]
    TextMeshProUGUI panelText;

    public void SetActive(bool active)
	{
        gameObject.SetActive(active);
	}

    public void SetText(string text)
	{
        panelText.text = text;
	}

    public void SetBackgroundColor(Color color)
	{
        background.color = color;
	}

    public void SetTextColor(Color color)
	{
        panelText.color = color;
	}
}
