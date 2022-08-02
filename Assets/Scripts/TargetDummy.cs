using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class TargetDummy : MonoBehaviour
{
    HealthComponent health;
    MeshRenderer meshRend;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthComponent>();
        meshRend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health.IsDead())
		{
            meshRend.material.color = Color.red;
		}
    }
}
