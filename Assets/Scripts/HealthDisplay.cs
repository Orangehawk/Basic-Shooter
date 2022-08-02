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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (healthComponent)
        {
            textPanel.SetText($"{prefix}: {healthComponent.GetHealthPercent()}%");
		}
    }
}
