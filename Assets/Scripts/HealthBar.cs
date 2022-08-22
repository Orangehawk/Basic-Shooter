using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    HealthComponent healthComponent;
    [SerializeField]
    public Canvas canvas;

    float currentVelocity;
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(canvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = Mathf.SmoothDamp(image.fillAmount, healthComponent.GetHealthPercent() / 100, ref currentVelocity, 0.05f);
    }
}
