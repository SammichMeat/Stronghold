using UnityEngine;

public abstract class SoldierBase : Damageable
{
    public string Class;
    public GameObject HomeBase;
    public GameObject EnemyStronghold;
    public int Damage;
    public int Value;
    // Update is called once per frame
    void Update()
    {
        
    }
    protected abstract void Attack();
    public override void Die()
    {
        Destroy(gameObject);
    }
}
