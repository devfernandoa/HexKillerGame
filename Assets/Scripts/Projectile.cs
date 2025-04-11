using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 1;
    public float lifetime = 2f; // Auto-destroy after this time
    public bool piercing = false; // Can hit multiple enemies

    [Header("Effects")]
    public GameObject hitEffect;

    [Header("Advanced Settings")]
    public bool ricochet = false;
    public bool homing = false;
    public int homingAmount = 1; // Number of times the projectile can retarget
    private Transform targetEnemy;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Auto-destroy after lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (homing)
        {
            FindNextTarget(); // Always find the closest target

            if (targetEnemy != null)
            {
                Vector2 direction = (targetEnemy.position - transform.position).normalized;
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * rb.linearVelocity.magnitude, Time.deltaTime * 5f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only interact with enemies
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                // Apply damage to enemy
                enemy.TakeDamage(damage);

                // Play hit effect if available
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }

                // Check if homing is enabled and there are remaining retargets
                if (homing && homingAmount > 0)
                {
                    homingAmount--;
                    FindNextTarget();
                    if (targetEnemy == null)
                    {
                        Destroy(gameObject); // Destroy if no more targets
                    }
                }
                else
                {
                    if (!piercing)
                    {
                        Destroy(gameObject); // Destroy if not piercing
                    }
                }
            }
        }
        // Check for ricochet on wall
        else if (collision.CompareTag("WallY") || collision.CompareTag("WallX"))
        {
            if (ricochet)
            {
                HandleRicochet(collision);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void FindNextTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy.transform;
        }
        else
        {
            targetEnemy = null;
        }
    }

    private void HandleRicochet(Collider2D collision)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (collision.CompareTag("WallY"))
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }
        else if (collision.CompareTag("WallX"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
        }
    }

    public void SetTarget(Transform enemy)
    {
        targetEnemy = enemy;
    }
}