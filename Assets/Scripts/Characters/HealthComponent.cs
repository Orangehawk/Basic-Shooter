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

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
	{
        return health;
	}

    public float GetHealthPercent()
    {
        return health / maxHealth * 100;
    }

    public event System.Action onDamage = delegate { };

    public event System.Action onHeal = delegate { };

    public event System.Action onDead = delegate { };

    public bool Heal(float amount)
	{
        if (health >= maxHealth)
            return false;

        health += amount;

        if(health > maxHealth)
		{
            health = maxHealth;
		}

        onHeal();
        return true;
    }

    public void Damage(float amount)
	{
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            onDead();
        }

        onDamage();
    }

    public void Kill()
	{
        health = 0;
        onDead();
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
