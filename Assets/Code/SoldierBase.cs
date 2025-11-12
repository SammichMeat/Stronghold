using UnityEngine;

public abstract class SoldierBase : Damageable
{
    public string Class;
    public GameObject HomeBase;
    public GameObject EnemyStronghold;
    public int Damage;
    public int Value;
    protected float AttackTimer;
    public float AttackCoolDown;

    public Vector3 Destination;
    public GameObject Target;
    public SpriteRenderer UnitCircle;
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
        if(Team == "Blue")
        {
            UnitCircle.color = Color.cyan;
        }
        else if(Team == "Red")
        {
            UnitCircle.color = Color.red;
        }

    }
    protected abstract void Attack();
    public override void Die()
    {
        Destroy(gameObject);
    }
}
