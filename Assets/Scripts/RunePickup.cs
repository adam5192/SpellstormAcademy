using UnityEngine;

public class RunePickup : MonoBehaviour
{
    public string runeType; // Fire, Ice, Lightning

    [Header("pickup sound")]
    public AudioClip pickupSfx;
    public float pickupVolume = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // add rune + xp
            other.GetComponent<PlayerController>()?.AddRune(runeType);

            if (pickupSfx != null)
                AudioHelper.PlayClipAtPointRandomized(pickupSfx, transform.position, pickupVolume, 0.96f, 1.04f);

            Destroy(gameObject);
        }
    }
}
