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
    protected void Update()
    {
        AttackTimer += Time.deltaTime;
        if(BowState == 0 && AttackTimer > AttackCoolDown / 6)
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
    protected override void Attack()
    {
        Rigidbody2D Arb = Arrow.GetComponent<Rigidbody2D>();
        Arb.AddForce(250 * Arrow.transform.right);
        BowParts[BowState].SetActive(false);
        BowState = 0;
        BowParts[BowState].SetActive(true);
        AttackTimer = 0;
        Destroy(Arrow, 5);
    }
}
