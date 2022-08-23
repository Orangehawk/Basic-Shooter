using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderToPercent : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    string prefix;

    // Update is called once per frame
    void Update()
    {
        text.text = $"{prefix}{slider.value / slider.maxValue * 100}%";
    }
}
