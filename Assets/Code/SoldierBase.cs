using UnityEngine;

public abstract class SoldierBase : Damageable
{
    public string Class;
    public GameObject HomeBase;
    public GameObject EnemyStronghold;
    public int Damage;
    public int Value;

    public Vector2 Destination;
    public GameObject Target;
    protected override void Start()
    {
        base.Start();
        GameObject[] Strongholds = GameObject.FindGameObjectsWithTag("Stronghold");
        foreach(GameObject Base in Strongholds)
        {
            Stronghold BaseCode = Base.GetComponent<Stronghold>();
            if (BaseCode != null)
            {
                if(BaseCode.Team == Team)
                {
                    HomeBase = Base;
                }
                else
                {
                    EnemyStronghold = Base;
                }
            }
        }

    }
    public void ChangeDestination()
    {
        
    }
    protected abstract void Attack();
    public override void Die()
    {
        Destroy(gameObject);
    }
}
