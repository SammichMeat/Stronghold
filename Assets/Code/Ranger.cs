using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class Ranger : SoldierBase
{
    public GameObject RangedAttack;
    public Transform SpawnSpot;
    public GameObject[] BowParts;
    private int BowState;
    private GameObject Arrow;

    public int WanderRange;
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
        if (BowState == 0 && AttackTimer > AttackCoolDown / 6)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 1 && AttackTimer > AttackCoolDown / 3)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 2 && AttackTimer > AttackCoolDown / 2)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
        }
        else if (BowState == 3 && AttackTimer > 2 * AttackCoolDown / 3)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
            Arrow = Instantiate(RangedAttack, SpawnSpot.position, SpawnSpot.rotation) as GameObject;
        }
        else if (BowState == 4 && AttackTimer > 5 * AttackCoolDown / 6)
        {
            BowParts[BowState].SetActive(false);
            BowState++;
            BowParts[BowState].SetActive(true);
            Attack();
        }
    }
    public void PatrolPoint()
    {
        WanderRange++;
        float WandX = HomeBase.transform.position.x;
        float WandY = HomeBase.transform.position.y;
        int RNG = Random.Range(0, 3);
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
        Rigidbody2D Arb = Arrow.GetComponent<Rigidbody2D>();
        Arb.AddForce(250 * Arrow.transform.right);
        BowParts[BowState].SetActive(false);
        BowState = 0;
        BowParts[BowState].SetActive(true);
        AttackTimer = -AttackCoolDown;
        Destroy(Arrow, 5);
    }
}
