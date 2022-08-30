using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damage(float amount);
    public bool Heal(float amount);
    public void Kill();
}
