using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Barbarian : SoldierBase
{
    public GameObject Axe;
    public Barbarian Self;
    protected List<GameObject> unitsInRange = new List<GameObject>();
    public Animator anime;

    [Header("Detection Settings")]
    public CircleCollider2D detectionCollider;
    [SerializeField] protected float detectionRange = 5f;  
    [SerializeField] protected float attackRange = 1.5f;

    protected override void Start()
    {
        base.Start();
        if (EnemyStronghold != null)
        {
            Target = EnemyStronghold;
            Destination = EnemyStronghold.transform.position;
        }
        if (detectionCollider != null)
        {
            {
                detectionCollider.radius = detectionRange;
            }
        }
        anime = GetComponentInChildren<Animator>();
    }

    protected void Update()
    {
        Charge();
        AttackTimer += Time.deltaTime;
       

    }

    protected void Charge()
    {
        
        if (Target != null && Target != EnemyStronghold)
        {
            Damageable targetDamageable = Target.GetComponent<Damageable>();
            if (targetDamageable == null)
            {
                Target = null;
            }
        }

        GameObject closestEnemy = GetClosestEnemy();

        if (closestEnemy != null)
        {
            Target = closestEnemy;
        }
        else if (Target == null || (Target != EnemyStronghold && Vector2.Distance(transform.position, Target.transform.position) > detectionRange))
        {
            Target = EnemyStronghold;
            if (EnemyStronghold != null)
            {
                Destination = EnemyStronghold.transform.position;
            }
        }

        if (Target == null)
        {
            AttackTimer = 0;
            if (EnemyStronghold != null && Destination == Vector3.zero)
            {
                Destination = EnemyStronghold.transform.position;
            }

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

            float distanceToTarget = Vector2.Distance(transform.position, Destination);

            // Chase if outside attack range
            if (distanceToTarget > attackRange)
            {
                rb.linearVelocity = Self.transform.right * MoveSpeed;
            }
            // Stop and attack if within attack range
            else
            {
                rb.linearVelocity = Vector2.zero;
                Attack();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Unit") || other.CompareTag("Stronghold"))
        {
            unitsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Unit") || other.CompareTag("Stronghold"))
        {
            unitsInRange.Remove(other.gameObject);
        }
    }

    protected List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach (GameObject unit in unitsInRange)
        {
            if (unit == null) continue;

            Damageable unitScript = unit.GetComponent<Damageable>();
            if (unitScript != null && unitScript.Team != Team)
            {
                enemies.Add(unit);
            }
        }
        return enemies;
    }

    protected GameObject GetClosestEnemy()
    {
        List<GameObject> enemies = GetEnemiesInRange();
        if (enemies.Count == 0) return null;

        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.GetComponent<Damageable>() == null) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    protected override void Attack()
    {
        if (AttackTimer < AttackCoolDown) return;

        if (Target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, Target.transform.position);
            if (distanceToTarget > attackRange) return;
            Damageable targetDamageable = Target.GetComponent<Damageable>();
            if (targetDamageable != null && targetDamageable.Team != Team)
            {
                targetDamageable.TakeDamage(Damage);
                AttackTimer = 0;
                Debug.Log($"{gameObject.name} attacked {Target.name} for {Damage} damage");
                anime.SetTrigger("Attack");
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); 
    }

    private void OnDrawGizmosSelected()
    {

        // Draw the attack range circle (orange)
        Gizmos.color = new Color(1f, 0.5f, 0f); // Orange
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw lines to enemies in range
        Gizmos.color = Color.red;
        foreach (GameObject enemy in GetEnemiesInRange())
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }
        }

        // Draw line to current target
        if (Target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }
    }



}



   


