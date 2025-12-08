using System.Collections.Generic;
using UnityEngine;

public class Claric : SoldierBase
{
    public List<GameObject> Units = new List<GameObject>();
    public Transform LookPointer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        ClassType = "Cleric";
        MoveSpeed = 2f;
        Units = HomeBase.GetComponent<Stronghold>().Soldiers;
        Wander();
    }
    // Update is called once per frame
    void Update()
    {
        AttackTimer += Time.deltaTime;
        Moving();
    }
    public override void Die()
    {
        Destroy(gameObject);
    }
    protected override void Attack() 
    {
        if(AttackTimer > AttackCoolDown)
        {
            Target.GetComponent<SoldierBase>().TakeDamage(-5);
            AttackTimer = -AttackCoolDown;
            Debug.Log($"Healed {Target.name} to {Target.GetComponent<SoldierBase>().Health}HP");
            if (Target.GetComponent<SoldierBase>().Health > Target.GetComponent<SoldierBase>().MaxHealth * .9f)
            {
                WeakestAlly();
            }
        }
    }
    public void Moving()
    {
        WeakestAlly();
        if (Target == null)
        {
            if (Vector3.Distance(transform.position, Destination) < .5f)
            {
                rb.linearVelocity = Vector2.zero;
                Wander();
            }
            else
            {
                LookAt(Destination);
                rb.linearVelocity = LookPointer.transform.right * MoveSpeed * .5f;
            }
        }
        else
        {
            Destination = Target.transform.position;
            Destination.x = Mathf.Max(Mathf.Min(Destination.x, 10), -10);
            Destination.y = Mathf.Max(Mathf.Min(Destination.y, 5), -5);
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
        if(location == gameObject || location.GetComponent<SoldierBase>().Health > location.GetComponent<SoldierBase>().MaxHealth * .9f)
        {
            return;
        }
        if(Target == null)
        {
            Target = location;
        }
        else if(Target != location)
        {
            float StayWeight = (Target.GetComponent<SoldierBase>().Health / Target.GetComponent<SoldierBase>().MaxHealth * Target.GetComponent<SoldierBase>().Health + Vector2.Distance(transform.position, Target.transform.position) / 5);
            float NewWeight = (location.GetComponent<SoldierBase>().Health / location.GetComponent<SoldierBase>().MaxHealth * location.GetComponent<SoldierBase>().Health + Vector2.Distance(transform.position, location.transform.position) / 5);
            if(NewWeight < StayWeight)
            {
                Target = location;
            }
        }
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
    public void WeakestAlly()
    {
        foreach(GameObject Soldier in Units)
        {
            try
            {
                Called(Soldier);
            }
            catch(UnityEngine.MissingReferenceException)
            {

            }
        }
    }
    public void Wander()
    {
        Destination = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0) + HomeBase.transform.position;
    }
}
