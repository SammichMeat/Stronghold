using System.Collections.Generic;
using UnityEngine;

public class Claric : SoldierBase
{
    List<GameObject> Units = new List<GameObject>();
    public Transform LookPointer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {

        base.Start();
        ClassType = "Cleric";
        MoveSpeed = 2f;
        Units = HomeBase.GetComponent<Stronghold>().Soldiers;
    }
    // Update is called once per frame
    void Update()
    {
        Moving();
    }
    public override void Die()
    {
        Destroy(gameObject);
    }
    protected override void Attack() 
    {
        Target.GetComponent<SoldierBase>().TakeDamage(-5);
    }
    public void Moving()
    {
        if (Target == null)
        {
            AttackTimer = 0;
            if (Vector3.Distance(transform.position, Destination) < .5f)
            {
                rb.linearVelocity = Vector2.zero;
                //PatrolPoint(); //Choose new destination
                Target = Units[0];
            }
            else
            {
                LookAt(Destination);
                rb.linearVelocity = LookPointer.transform.right * .75f * MoveSpeed;
            }
        }
        else
        {
            Destination = Target.transform.position;
            LookAt(Target.transform.position);
            if (Vector2.Distance(transform.position, Destination) > 1.5f)
            {
                rb.linearVelocity = LookPointer.transform.right * MoveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                Attack();
            }
        }
    }
    public void Called(GameObject location)
    {
        Target = location;
        Debug.Log("GotCalled");
    }
    public void Heal(float activeHp, int maxHP)
    {
        activeHp = maxHP;
    }
    protected override void LookAt(Vector2 point)
    {
        float angle = AngleBetweenPoints(point, LookPointer.transform.position);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        LookPointer.transform.rotation = Quaternion.Slerp(LookPointer.transform.rotation, targetRotation, Time.deltaTime);
    }
}
