using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;
    public bool isPlayerTower = true;
    public int attackDamage = 10;
    public float attackCooldown = 1.2f;
    public float attackRange = 6f;

    float attackTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            // find nearest enemy unit
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
            Transform best = null;
            float bestd = Mathf.Infinity;
            foreach (var c in hits)
            {
                Unit u = c.GetComponent<Unit>();
                if (u != null && u.isPlayerUnit != isPlayerTower)
                {
                    float d = Vector3.Distance(transform.position, u.transform.position);
                    if (d < bestd) { bestd = d; best = u.transform; }
                }
            }
            if (best != null)
            {
                best.GetComponent<Unit>().TakeDamage(attackDamage);
                attackTimer = attackCooldown;
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
