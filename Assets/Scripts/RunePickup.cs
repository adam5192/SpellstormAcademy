using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunePickup : MonoBehaviour
{
    public string runeType = "Fire";
    public float floatSpeed = 2f;

    private void Update()
    {
        // Floating effect
        transform.position += Vector3.up * Mathf.Sin(Time.time * floatSpeed) * 0.0005f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().CollectRune(runeType);
            Destroy(gameObject);
        }
    }
}

