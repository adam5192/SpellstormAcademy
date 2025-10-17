using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;          // enemy speed
    private Transform player;             // player reference
    public GameObject runePrefab;         // what to drop when dead
    private float attackCooldown = 1f;    // delay between hits
    private float attackTimer = 0f;       // timer for that delay
    private float stopDistance = 0.5f;    // how close to stop from player

    void Start()
    {
        // find the player once at start
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveSpeed += Random.Range(-0.4f, 0.4f); // tiny speed variance
    }

    void Update()
    {
        if (player == null) return;

        Vector2 pos = transform.position;
        Vector2 dirToPlayer = ((Vector2)player.position - pos).normalized;

        // small jitter to make paths less robotic
        dirToPlayer = (dirToPlayer + Random.insideUnitCircle * 0.1f).normalized;

        // seperation force to keep space between enemies
        Vector2 separation = Vector2.zero;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(pos, 0.6f); // enemy size
        foreach (var c in nearby)
        {
            if (c.CompareTag("Enemy") && c.gameObject != gameObject)
            {
                Vector2 away = (pos - (Vector2)c.transform.position);
                separation += away.normalized / Mathf.Max(away.magnitude, 0.1f);
            }
        }

        // combine chase + separation
        Vector2 finalDir = (dirToPlayer + separation * 0.5f).normalized;

        // move if not too close to player
        float dist = Vector2.Distance(pos, player.position);
        if (dist > stopDistance)
            transform.position = pos + finalDir * moveSpeed * Time.deltaTime;

        // attack cooldown tick
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // hit the player
        if (collision.CompareTag("Player") && attackTimer <= 0f)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1f);
                attackTimer = attackCooldown; // wait before next hit
            }
        }
    }
}
