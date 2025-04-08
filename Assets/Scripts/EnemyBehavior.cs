using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 5f;
    public float speed = 2f;
    public float waitTime = 0.5f;
    public int health = 1;

    [Header("Effects")]
    public GameObject deathEffect;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private float fadeTimer;
    private bool isChasing = false;

    // Reference to SpawnerColetavel
    private SpawnerColetavel spawner;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeTimer = fadeDuration;

        // Find the SpawnerColetavel in the scene
        spawner = FindObjectOfType<SpawnerColetavel>();

        StartCoroutine(StartChasingAfterDelay(waitTime));
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Visual feedback for hit
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Notify any systems (score, etc.)
        Destroy(gameObject);

        // Spawn collectable using the spawner instance
        if (spawner != null)
        {
            spawner.SpawnCollectable(transform.position);
        }
    }

    private IEnumerator StartChasingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isChasing = true;
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }

        FadeOut();
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void FadeOut()
    {
        fadeTimer -= Time.deltaTime;

        float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

        spriteRenderer.color = new Color(0.7f, 0f, 0f, alpha);

        if (fadeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOverScreen();
            }

            Destroy(collision.gameObject);
        }
    }
}