using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITextPanel : MonoBehaviour
{
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
}
