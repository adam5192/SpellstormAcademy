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
    }

    void Update()
    {
        if (player == null) return;

        // move toward player but stop when close enough
        float distance = Vector2.Distance(player.position, transform.position);
        if (distance > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        // tick down cooldown
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
