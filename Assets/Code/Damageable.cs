using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public int Health;
    public int MaxHealth;
    public string Team;
    public float DamageResistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Health = MaxHealth;
    }
    public void TakeDamage(int damage)
    {
        Health -= Mathf.RoundToInt(damage * (1 - DamageResistance));
        if (Health < 0)
        {
            Die();
        }
        else if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    public abstract void Die();
}
