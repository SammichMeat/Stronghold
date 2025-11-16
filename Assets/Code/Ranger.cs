using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Burst.Intrinsics.X86;

public class Ranger : SoldierBase
{
    public GameObject RangedAttack;
    public Transform SpawnSpot;
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
        Ray2D[] VisionCone = new Ray2D[40];
        for(int i = 0; i < VisionCone.Length; i++)
        {
            Debug.DrawRay(transform.position + transform.right/7, (transform.right * 5 + transform.up * (i - 20)/4), UnitCircle.color);
            RaycastHit2D HitObject = Physics2D.Raycast(transform.position + transform.right/7, (transform.right * 5 + transform.up * (i - 20)/4), 5, UnitVision);
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
                    Debug.Log($"{gameObject.name} has seen {SeenObject.name}");
                    if (Seen == null)
                    {
                        continue;
                    }
                    if(Seen.Team != Team)
                    {
                        WanderRange = Mathf.RoundToInt(Mathf.Min(WanderRange, Vector2.Distance(HomeBase.transform.position, SeenObject.transform.position)));
                        Debug.Log($"{gameObject.name} Has spotted an enemy");
                        return SeenObject;
                    }
                }
                catch(System.Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        return null;
    }
    private void ActivePatrol()
    {
        Target = VisionCone();
        if (Target == null)
        {
            AttackTimer = 0;
            BowState = 0;
            if (Vector3.Distance(transform.position, Destination) < .5f)
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
            Destination = Target.transform.position;
            LookAt(Target.transform.position);
            if (Vector3.Distance(transform.position, Destination) > 3)
            {
                rb.linearVelocity = transform.right *  1.5f * MoveSpeed;
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
            BowState++;
            BowParts[BowState].SetActive(true);
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
            WandX += WanderRange/2f;
        }
        else if(RNG == 1)
        {
            WandX -= WanderRange/2f;
        }
        RNG = Random.Range(0, 3);
        if (RNG == 0)
        {
            WandY += WanderRange /2f;
        }
        else if(RNG == 1)
        {
            WandY -= WanderRange /2f;
        }
        WandX = Mathf.Max(Mathf.Min(WandX, 10), -10);
        WandY = Mathf.Max(Mathf.Min(WandY, 5), -5);
        Destination = new Vector2(WandX, WandY);
    }
    protected override void Attack()
    {
        Arrow.GetComponent<Arrow>().Fire(Team);
        BowParts[BowState].SetActive(false);
        BowState = 0;
        BowParts[BowState].SetActive(true);
        AttackTimer = -AttackCoolDown;
        Destroy(Arrow, 5);
    }
}
