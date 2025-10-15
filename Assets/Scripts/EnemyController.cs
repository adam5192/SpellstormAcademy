using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    public GameObject runePrefab;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Move toward player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy if hit by projectile
        if (collision.CompareTag("Projectile"))
        {
            Instantiate(runePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
