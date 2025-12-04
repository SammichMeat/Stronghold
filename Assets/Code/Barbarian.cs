using UnityEngine;

public class Barbarian : SoldierBase
{
    public float AttackRange = 1.2f;
    public float VisionRange = 4f;
    public float VisionAngle = 60f;       // total angle of the vision cone
    public LayerMask UnitVision;

    public int WanderRange = 1;
    public bool inHelpRange = false;

    private GameObject Self;
    

    protected override void Start()
    {
        base.Start();
        Self = gameObject;
        PatrolPoint();
        Damage = 15;
    }

    private void Update()
    {
        AttackTimer += Time.deltaTime;

        // Find target using ray-based vision cone
        if (Target == null)
            Target = ScanForEnemies();

        // Lost target?
        if (Target != null && Vector2.Distance(transform.position, Target.transform.position) > VisionRange)
            Target = null;

        if (Target == null)
        {
            // Patrol
            MoveTowards(Destination);
            if (Vector2.Distance(transform.position, Destination) < 0.5f)
                PatrolPoint();
        }
        else
        {
            // Chase target
            Destination = Target.transform.position;
            float dist = Vector2.Distance(transform.position, Destination);

            LookAt(Destination);

            if (dist > AttackRange)
            {
                MoveTowards(Destination);
            }
            else
            {
                rb.velocity = Vector2.zero;
                TryAttack();
            }
        }

        HealthCheck();
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;
        rb.velocity = direction * MoveSpeed;
    }

    private GameObject ScanForEnemies()
    {
        GameObject closest = null;
        float closestDist = float.MaxValue;

        int rayCount = 30; // number of rays in the cone
        float halfAngle = VisionAngle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -halfAngle + (VisionAngle / rayCount) * i;
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, VisionRange, UnitVision);
            Debug.DrawRay(transform.position, rayDir * VisionRange, Color.yellow);

            if (hit)
            {
                Damageable dmg = hit.collider.GetComponentInParent<Damageable>();
                if (dmg != null && dmg.Team != Team)
                {
                    float dist = Vector2.Distance(transform.position, dmg.transform.position);
                    if (dist < closestDist)
                    {
                        closest = dmg.gameObject;
                        closestDist = dist;
                    }
                }
            }
        }

        return closest;
    }

    private void TryAttack()
    {
        if (AttackTimer < AttackCoolDown)
            return;

        AttackTimer = 0;

        if (Target != null)
        {
            Damageable dmg = Target.GetComponentInParent<Damageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(Damage);
            }
        }
    }

    public void HealthCheck()
    {
        if (Self != null && Self.GetComponent<Damageable>().Health <= MaxHealth / 2 && inHelpRange)
        {
            // CallCleric(); // optional
        }
    }

    public void PatrolPoint()
    {
        WanderRange++;
        float WandX = HomeBase.transform.position.x + Random.Range(-WanderRange / 2f, WanderRange / 2f);
        float WandY = HomeBase.transform.position.y + Random.Range(-WanderRange / 2f, WanderRange / 2f);

        WandX = Mathf.Clamp(WandX, -10f, 10f);
        WandY = Mathf.Clamp(WandY, -5f, 5f);

        Destination = new Vector2(WandX, WandY);
    }

    protected override void Attack()
    {
        AttackTimer = -AttackCoolDown;
    }

    protected override void LookAt(Vector2 point)
    {
        Vector2 dir = point - (Vector2)transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // Draw vision cone
        Gizmos.color = Color.yellow;
        Vector2 left = Quaternion.Euler(0, 0, -VisionAngle / 2) * transform.right * VisionRange;
        Vector2 right = Quaternion.Euler(0, 0, VisionAngle / 2) * transform.right * VisionRange;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + left);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + right);
    }
}




