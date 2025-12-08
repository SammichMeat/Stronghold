using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Burst.Intrinsics.X86;

public class Ranger : SoldierBase
{
    public GameObject RangedAttack;
    public Transform SpawnSpot;
    public GameObject Bow;
    public GameObject[] BowParts;
    private int BowState;
    private GameObject Arrow;

    public int WanderRange;
    public LayerMask UnitVision;
    protected override void Start()
    {
        base.Start();
        WanderRange = 1;
        PatrolPoint();
    }

    protected void Update()
    {
        ActivePatrol();
        AttackTimer += Time.deltaTime;
        AnimationStuff();
    }
    private GameObject VisionCone()
    {
        GameObject Closest = null;
        Ray2D[] VisionCone = new Ray2D[40];
        for(int i = 0; i < VisionCone.Length; i++)
        {
            Debug.DrawRay(Bow.transform.position + Bow.transform.right/7, (Bow.transform.right * 5 + Bow.transform.up * (i - 20)/4), UnitCircle.color);
            RaycastHit2D HitObject = Physics2D.Raycast(transform.position + Bow.transform.right/7, (Bow.transform.right * 5 + Bow.transform.up * (i - 20)/4), 5, UnitVision);
            if(HitObject)
            {
                GameObject SeenObject = HitObject.collider.gameObject;
                try
                {
                    Damageable Seen = SeenObject.GetComponent<Damageable>();
                    while(Seen == null && SeenObject.transform.parent != null)
                    {
                        SeenObject = SeenObject.transform.parent.gameObject;
                        Seen = SeenObject.GetComponent<Damageable>();
                    }
                    //Debug.Log($"{gameObject.name} has seen {SeenObject.name}");
                    if (Seen == null)
                    {
                        continue;
                    }
                    if(Seen.Team != Team)
                    {
                        //Debug.Log($"{gameObject.name} Has spotted an enemy");
                        Closest = ClosestChoice(SeenObject, Closest);
                    }
                    else if(SeenObject != HomeBase)
                    {
                        SoldierBase Ally = SeenObject.GetComponent<SoldierBase>();
                        if (Vector2.Distance(transform.position, Ally.transform.position) < 2 && Target == null)
                        {
                            LookAt(transform.position + Bow.transform.right - Bow.transform.up);
                        }
                        if (Ally.ClassType.ToLower() != "cleric" && Ally.ClassType.ToLower() != "claric")
                        {
                            Closest = ClosestChoice(Ally.Target, Closest);
                            if(Ally.ClassType.ToLower() == "ranger")
                            {
                                Ranger AllyR = SeenObject.GetComponent<Ranger>();
                                RangeExpansion(AllyR);
                            }
                        }
                    }
                }
                catch(System.Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        if (Closest != null)
        {
            WanderRange = Mathf.RoundToInt(Mathf.Min(WanderRange, Vector2.Distance(HomeBase.transform.position, Closest.transform.position)));
        }
        return Closest;
    }
    private void ActivePatrol()
    {
        Target = VisionCone();
        if (Target == null)
        {
            AttackTimer = 0;
            if(BowState > 0)
            {
                BowParts[BowState].SetActive(false);
                BowState = 0;
                BowParts[BowState].SetActive(true);
            }
            if (Vector3.Distance(transform.position, Destination) < .5f)
            {
                rb.linearVelocity = Vector2.zero;
                PatrolPoint();
            }
            else
            {
                LookAt(Destination);
                rb.linearVelocity = Bow.transform.right * MoveSpeed;
            }
        }
        else
        {
            Destination = Target.transform.position;
            LookAt(Target.transform.position);
            if (Vector2.Distance(transform.position, Destination) > 4)
            {
                rb.linearVelocity = Bow.transform.right *  1.5f * MoveSpeed;
            }
            else if(Vector2.Distance(transform.position, Destination) < 2)
            {
                rb.linearVelocity = Bow.transform.right * -.5f * MoveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    private void AnimationStuff()
    {
        if (Target == null)
        {
            AttackTimer = 0;
            BowState = 0;
            if (Arrow != null)
            {
                Destroy(Arrow);
            }
        }
        else if (BowState == 0 && AttackTimer > AttackCoolDown / 5)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 1 && AttackTimer > 2 * AttackCoolDown / 5)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 2 && AttackTimer > 3 * AttackCoolDown / 5)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 3 && AttackTimer > 4 * AttackCoolDown / 5)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
            Arrow = Instantiate(RangedAttack, SpawnSpot) as GameObject;
        }
        else if (BowState == 4 && AttackTimer > AttackCoolDown)
        {
            LookAt(Target.transform.position);
            BowParts[BowState].SetActive(false);
            Attack();
        }
    }
    public void PatrolPoint()
    {
        if(Mathf.Abs(Destination.x) - Mathf.Abs(HomeBase.transform.position.x) < 0)
        {
            WanderRange++;
        }
        float WandX = HomeBase.transform.position.x;
        float WandY = HomeBase.transform.position.y;
        int RNG = Random.Range(0, 2);
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
        RaycastHit2D HitObject = Physics2D.Raycast(transform.position + Bow.transform.right / 7, Bow.transform.right, 5, UnitVision);
        if (HitObject)
        {
            GameObject SeenObject = HitObject.collider.gameObject;
            try
            {
                Damageable Seen = SeenObject.GetComponent<Damageable>();
                while (Seen == null && SeenObject.transform.parent != null)
                {
                    SeenObject = SeenObject.transform.parent.gameObject;
                    Seen = SeenObject.GetComponent<Damageable>();
                }
                //Debug.Log($"{gameObject.name} has seen {SeenObject.name}");
                if (Seen == null)
                {
                    return;
                }
                if (Seen.Team != Team)
                {
                    Arrow.GetComponent<Arrow>().Fire(Team);
                    BowParts[BowState].SetActive(false);
                    BowState = 0;
                    BowParts[BowState].SetActive(true);
                    AttackTimer = -AttackCoolDown;
                    Destroy(Arrow, 5);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
    }
    public GameObject ClosestChoice(GameObject Object1, GameObject Object2)
    {
        if(Object1 == null)
        {
            return Object2;
        }
        else if(Object2 == null)
        {
            return Object1;
        }
        float Distance1 = Vector2.Distance(Object1.transform.position, transform.position);
        float Distance2 = Vector2.Distance(Object2.transform.position, transform.position);
        if(Distance1 < Distance2)
        {
            return Object1;
        }
        else
        {
            return Object2;
        }
    }
    public void RangeExpansion(Ranger FriendlyRanger)
    {
        if(FriendlyRanger != null)
        {
            if(FriendlyRanger.WanderRange < WanderRange)
            {
                FriendlyRanger.RangeExpansion(this);
            }
            else
            {
                WanderRange = FriendlyRanger.WanderRange;
            }
        }
    }
    protected override void LookAt(Vector2 point)
    {
        float angle = AngleBetweenPoints(point, Bow.transform.position);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        Bow.transform.rotation = Quaternion.Slerp(Bow.transform.rotation, targetRotation, Time.deltaTime);
    }
    public override void TakeDamage(int Dmg)
    {
        base.TakeDamage(Dmg);
        if (Target != EnemyStronghold)
        {
            Destination = transform.position - Bow.transform.right;
        }
    }
}
