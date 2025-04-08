using UnityEngine;
using System.Collections;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;
    public float currentSpeed;
    private Rigidbody2D rb;
    private Vector2 movement;
    private AudioSource audioSource;

    [Header("Combat")]
    public bool canShoot = false;
    public float shootCooldown = 2f;
    private float shootTimer = 0f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Power-Up Effects")]
    public float temporarySpeedBoost = 0f;
    public float temporarySpeedDuration = 0f;
    private float speedBoostTimer = 0f;

    // ===== MOVEMENT ABILITIES =====
    [Header("Dash Settings")]
    public float dashSpeedMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldownTime = 1f;
    public bool canDash = true;
    private Vector2 dashDirection;

    [Header("Momentum Settings")]
    public float maxMomentumMultiplier = 2f;
    public float momentumBuildRate = 0.1f;
    private float currentMomentum = 1f;
    private Vector2 lastMovementDirection;

    // ===== PROJECTILE ABILITIES =====
    [Header("Homing Shot Settings")]
    public float homingAmount = 1f;
    public bool homingEnabled = false;

    [Header("Multi-Shot Settings")]
    public bool multiShotEnabled = false;
    public int multiShotAmount = 1;
    private float multiShotDelay = 0.1f;

    [Header("Split Shot Settings")]
    public bool splitShotEnabled = false;
    public int splitShotAmount = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currentSpeed = baseSpeed;
    }

    private void Start()
    {
        if (XpManager.Instance != null)
        {
            XpManager.Instance.SetPlayerAlive(true);
        }
    }

    private void Update()
    {
        HandleMovementInput();

        // Automatic shooting logic
        if (canShoot)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                AutoShootAtNearestEnemy();
                shootTimer = shootCooldown;
            }
        }

        // Handle temporary speed boost
        if (temporarySpeedBoost > 0)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0)
            {
                currentSpeed = baseSpeed;
                temporarySpeedBoost = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector2(moveHorizontal, moveVertical);

        // Dash implementation
        if (canDash && Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0.1f)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        float originalSpeed = currentSpeed;
        currentSpeed = baseSpeed * dashSpeedMultiplier;

        yield return new WaitForSeconds(dashDuration);

        currentSpeed = originalSpeed;
        StartCoroutine(DashCooldown());
    }

    private void AutoShootAtNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

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
            Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;

            for (int i = 0; i < splitShotAmount; i++)
            {
                // Calculate the angle for split shots
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle += (360 / splitShotAmount) * i;
                Vector2 newDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.right;

                // Fire multiple shots in the same direction with delay
                StartCoroutine(FireMultiShot(newDirection));
            }
        }
    }

    // Wait for the delay between shots
    private IEnumerator FireMultiShot(Vector2 direction)
    {
        for (int j = 0; j < multiShotAmount; j++)
        {
            // Instantiate and shoot the projectile
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D newProjectileRb = newProjectile.GetComponent<Rigidbody2D>();

            newProjectileRb.linearVelocity = direction * projectileSpeed;

            // Rotate the projectile to face the direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            newProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Wait for the delay before firing the next shot
            yield return new WaitForSeconds(multiShotDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coletavel"))
        {
            audioSource.Play();
            Destroy(other.gameObject);
            ScoreManager.Instance.AddScore(1);
        }
    }

    private void Die()
    {
        if (XpManager.Instance != null)
        {
            XpManager.Instance.SetPlayerAlive(false);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (XpManager.Instance != null)
        {
            XpManager.Instance.SetPlayerAlive(false);
        }
    }

    // Power-up methods
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        temporarySpeedBoost = multiplier;
        speedBoostTimer = duration;
    }

    public void ModifyShootCooldown(float multiplier)
    {
        shootCooldown *= multiplier;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }

    // ===== MOVEMENT ABILITIES =====
    public void EnableDashAbility()
    {
        canDash = true;
    }
    private IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldownTime);
        canDash = true;
    }

    public void EnableMomentum()
    {
        StartCoroutine(MomentumBuildUp());
    }

    private IEnumerator MomentumBuildUp()
    {
        while (true)
        {
            if (movement.magnitude > 0.1f)
            {
                if (movement == lastMovementDirection)
                {
                    currentMomentum = Mathf.Min(currentMomentum + momentumBuildRate * Time.deltaTime, maxMomentumMultiplier);
                }
                else
                {
                    currentMomentum = 1f; // Reset if direction changes
                }
                lastMovementDirection = movement;
            }
            else
            {
                currentMomentum = 1f; // Reset when not moving
            }

            currentSpeed = baseSpeed * currentMomentum;
            yield return null;
        }
    }

    // ===== PROJECTILE ABILITIES =====
    public void EnableBasicShooting()
    {
        canShoot = true;
        projectilePrefab.GetComponent<Projectile>().piercing = false;
        projectilePrefab.GetComponent<Projectile>().ricochet = false;
        projectilePrefab.GetComponent<Projectile>().homing = false;
    }

    public void EnablePiercingShots()
    {
        canShoot = true;
        projectilePrefab.GetComponent<Projectile>().piercing = true;
    }

    public void EnableRicochetShots()
    {
        canShoot = true;
        projectilePrefab.GetComponent<Projectile>().ricochet = true;
    }

    public void EnableHomingShots()
    {
        canShoot = true;
        projectilePrefab.GetComponent<Projectile>().homing = true;
        projectilePrefab.GetComponent<Projectile>().homingAmount = (int)homingAmount;
    }

    public void EnableMultiShot()
    {
        canShoot = true;
        multiShotEnabled = true;
        multiShotAmount++;
    }

    public void EnableSplitShot()
    {
        canShoot = true;
        splitShotEnabled = true;
        splitShotAmount++;
    }
}