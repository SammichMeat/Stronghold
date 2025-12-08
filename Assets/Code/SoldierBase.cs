using UnityEngine;

public abstract class SoldierBase : Damageable
{
    public string ClassType;
    public GameObject HomeBase;
    public GameObject EnemyStronghold;
    public int Damage;
    public int Value;
    [SerializeField] protected float AttackTimer;
    public float AttackCoolDown;

    public Vector3 Destination;
    public GameObject Target;
    public SpriteRenderer UnitCircle;
    public float MoveSpeed;
    protected Rigidbody2D rb;
    public Transform LookPointer;
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
            //gameObject.layer = 6;
        }
        else if(Team == "Red")
        {
            UnitCircle.color = Color.red;
            //gameObject.layer = 7;
        }
        rb = GetComponent<Rigidbody2D>();
    }
    protected abstract void Attack();
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Health <= MaxHealth / 3 && ClassType != "Cleric")
        {
            CallingHelp();
        }
    }
    //Calling help from the Cleric
    public void CallingHelp()
    {
        foreach (GameObject Soldier in HomeBase.GetComponent<Stronghold>().Soldiers)
        {
            try
            {
                SoldierBase SoldierCode = Soldier.GetComponent<SoldierBase>();
                if (SoldierCode.ClassType == "Cleric")
                {
                    Soldier.BroadcastMessage("Called", gameObject);
                    Debug.Log("Calling Help!");
                }
            }
            catch(UnityEngine.MissingReferenceException)
            {

            }
        }
    }
    public override void Die()
    {
        HomeBase.GetComponent<Stronghold>().Soldiers.Remove(gameObject);
        Destroy(gameObject);
    }
    protected virtual void LookAt(Vector2 point)
    {
        float angle = AngleBetweenPoints(point, LookPointer.transform.position);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        LookPointer.transform.rotation = Quaternion.Slerp(LookPointer.transform.rotation, targetRotation, 3 * Time.deltaTime);
    }
    protected float AngleBetweenPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
