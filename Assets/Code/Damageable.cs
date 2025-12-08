using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public string Team;
    public float DamageResistance;
    public Transform HealthBar;
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
            HealthBar.transform.localScale = new Vector3(Health/MaxHealth, 1, 1);
        }
    }
    public abstract void Die();
}
