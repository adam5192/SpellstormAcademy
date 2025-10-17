using UnityEngine;

public class RunePickup : MonoBehaviour
{
    public string runeType; // Fire, Ice, Lightning

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // add rune + xp
            other.GetComponent<PlayerController>()?.AddRune(runeType);
            Destroy(gameObject);
        }
    }
}
