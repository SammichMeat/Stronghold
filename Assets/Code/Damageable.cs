using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public float Health;
    public int MaxHealth;
    public string Team;
    public float DamageResistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Health = MaxHealth;
    }
    public virtual void TakeDamage(int damage)
    {
        Health -= damage * (1 - DamageResistance);
        if (Health < 0)
        {
            Die();
        }
        else if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        else
        {
            //Adjust Health Bar
        }
    }
    public abstract void Die();
}
