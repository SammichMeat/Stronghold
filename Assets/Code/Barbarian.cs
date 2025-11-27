using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class Barbarian : SoldierBase
{
    private GameObject Axe;
    public Barbarian Self;
    public int WanderRange;
    public float BarbRange;
    protected float HelpTimer;
    public bool inHelpRange;
    protected override void Start()
    {
        base.Start();
        WanderRange = 1;
        PatrolPoint();
    }

    protected void Update()
    {

        AttackTimer += Time.deltaTime;
        if (Target == null && AttackTimer > 0)
        {
            AttackTimer = 0;
        }
        if (AttackTimer <= 0)
        {
            if (Vector3.Distance(transform.position, Destination) < 1)
            {
                rb.linearVelocity = Vector2.zero;
                PatrolPoint();
            }
            else
            {
                LookAt(Destination);
                rb.linearVelocity = transform.right * MoveSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        HealthCheck();

       
    }

    public void HealthCheck()
    {
        if (Self.Health <= MaxHealth /2 && HelpTimer > 0 && inHelpRange == true)
        {
            //CallCleric(); See Below
        }
    }

    /* Commented Out, Unity wants to do safe mode otherwise, still WIP
    public void CallCleric()
    {

        foreach (Damageable troops  in BarbRange)
        {
            GameObject cleric = troops.GetComponent<ClassType>();
            if (ClassType != null)
            {
                if (ClassType == "Cleric"  && troops.Team == Self.Team)
                {
                    // Request Heal from Cleric-  & reset heal timer
                }
                else
                {
                    //Ignore them, might be kinda funny for Barbs to chase down enemy clerics if their health is low, but not exactly a good tatic 
                }
            }
        }



    }
    */

    public void PatrolPoint() // Uses the Rangers currently, needs to be tweaked
    {
        WanderRange++;
        float WandX = HomeBase.transform.position.x;
        float WandY = HomeBase.transform.position.y;
        int RNG = Random.Range(0, 3);
        if (RNG == 0)
        {
            WandX += WanderRange / 2f;
        }
        else if (RNG == 1)
        {
            WandX -= WanderRange / 2f;
        }
        RNG = Random.Range(0, 3);
        if (RNG == 0)
        {
            WandY += WanderRange / 2f;
        }
        else if (RNG == 1)
        {
            WandY -= WanderRange / 2f;
        }
        WandX = Mathf.Max(Mathf.Min(WandX, 10), -10);
        WandY = Mathf.Max(Mathf.Min(WandY, 5), -5);
        Destination = new Vector2(WandX, WandY);
    }
    protected override void Attack()
    {
       
        AttackTimer = -AttackCoolDown;

    }
}


