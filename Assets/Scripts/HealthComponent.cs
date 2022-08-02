using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    float maxHealth = 100;
    [SerializeField]
    float health = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float GetHealth()
	{
        return health;
	}

    public float GetHealthPercent()
    {
        return health / maxHealth * 100;
    }

    public void Heal(float amount)
	{
        health += amount;

        if(health > maxHealth)
		{
            health = maxHealth;
		}
	}

    public void Damage(float amount)
	{
        health -= amount;

        if (health < 0)
        {
            health = 0;
        }
    }

    public bool IsDead()
	{
        if(health == 0)
		{
            return true;
		}
        else
		{
            return false;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
