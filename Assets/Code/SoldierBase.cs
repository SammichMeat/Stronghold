using UnityEngine;

public abstract class SoldierBase : MonoBehaviour
{
    public string Team;
    public string Class;
    public int Health;
    public int MaxHealth;
    public GameObject HomeBase;
    public GameObject EnemyStronghold;
    public int Damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected abstract void Attack();
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Die();
        }
        else if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
