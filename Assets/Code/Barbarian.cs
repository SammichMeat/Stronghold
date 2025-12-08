using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Barbarian : SoldierBase
{
    public GameObject Axe;
    public Barbarian Self;
    protected float HelpTimer;
    protected List<GameObject> unitsInRange = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        if (EnemyStronghold != null)
        {
            Target = EnemyStronghold;
        }

    }

    protected void Charge()
    {

        if (Target == null)
        {
            AttackTimer = 0;
            if (Vector3.Distance(transform.position, Destination) < .5f)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                LookAt(Destination);
                rb.linearVelocity = Self.transform.right * MoveSpeed;
            }
        }
        else
        {
            Destination = Target.transform.position;
            LookAt(Target.transform.position);
            if (Vector2.Distance(transform.position, Destination) > 1.5f)
            {
                rb.linearVelocity = Self.transform.right * MoveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                Attack();
                Debug.Log($"Attacked");
            }
        }

    }

    protected void Update()
    {
        Charge();
        HealthCheck();

    }

    public void HealthCheck()
    {
        if (Self.Health <= MaxHealth / 2 && HelpTimer > 0 == true)
        {
            //CallCleric(); See Below
        }
    }



  /*  private void OnTriggerEnter2D(Collider2D other)
    {
        string cType = GetComponentInParent<ClassType>;
        if (cType = "Cleric" || "Ranger" || "Barbarian") // Make sure units are tagged
        {
            unitsInRange.Add(other.gameObject);
        }
    }
  */

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Unit"))
        {
            unitsInRange.Remove(other.gameObject);
        }
    }



    // Helper methods remain the same
    protected List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach (GameObject unit in unitsInRange)
        {
            //var unitScript = unit.GetComponent<YourUnitScript>();
           // if (unitScript != null && unitScript.Team != Team)
            //{
            //    enemies.Add(unit);
           // }
        }
        return enemies;
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


    protected override void Attack()
    {
       
        AttackTimer = -AttackCoolDown;

    }
}



