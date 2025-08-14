using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public int attackDamage = 2;
    public float speed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public bool isPlayerUnit = true;

    private float attackTimer = 0f;
    private Transform currentTarget;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(int hp, int dmg, bool isPlayer)
    {
        maxHealth = hp;
        currentHealth = hp;
        attackDamage = dmg;
        isPlayerUnit = isPlayer;

        // Slightly vary appearance
        transform.localScale = Vector3.one * (1f + Random.Range(-0.1f, 0.2f));
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        // simple targeting: find nearest enemy unit, else move forward
        FindTarget();

        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            if (dist <= attackRange)
            {
                if (attackTimer <= 0f)
                {
                    Attack();
                    attackTimer = attackCooldown;
                }
            }
            else
            {
                MoveTowards(currentTarget.position);
            }
        }
        else
        {
            // move toward enemy side (z axis or x axis depending on your scene)
            Vector3 dir = isPlayerUnit ? Vector3.forward : Vector3.back;
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }

    void MoveTowards(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void FindTarget()
    {
        // search for nearest enemy unit or tower within a modest radius
        Collider[] hits = Physics.OverlapSphere(transform.position, 6f);
        float best = Mathf.Infinity;
        Transform bestT = null;
        foreach (var c in hits)
        {
            if (c.gameObject == this.gameObject) continue;
            // check if unit
            Unit other = c.GetComponent<Unit>();
            Tower t = c.GetComponent<Tower>();
            if (other != null && other.isPlayerUnit != this.isPlayerUnit)
            {
                float d = Vector3.Distance(transform.position, other.transform.position);
                if (d < best) { best = d; bestT = other.transform; }
            }
            else if (t != null && t.isPlayerTower != this.isPlayerUnit) // tower belongs to enemy
            {
                float d = Vector3.Distance(transform.position, t.transform.position);
                if (d < best) { best = d; bestT = t.transform; }
            }
        }
        currentTarget = bestT;
    }

    void Attack()
    {
        if (currentTarget == null) return;
        Unit other = currentTarget.GetComponent<Unit>();
        Tower t = currentTarget.GetComponent<Tower>();
        if (other != null) other.TakeDamage(attackDamage);
        if (t != null) t.TakeDamage(attackDamage);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
