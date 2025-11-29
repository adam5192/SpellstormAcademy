using UnityEngine;

// applies upgrades to player stats
public class UpgradeManager : MonoBehaviour
{
    [Header("rate multipliers")]
    // 1 = default, 1.15 = +15%
    public float fireRateMultiplier = 1f;
    public float iceRateMultiplier = 1f;
    public float lightningRateMultiplier = 1f;

    [Header("refs")]
    public PlayerController player; // auto filled if left empty

    void Awake()
    {
        // grab player on same object if not set
        if (player == null)
            player = GetComponent<PlayerController>();
    }

    // called by upgrade menu when player picks an upgrade
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogWarning("upgrade manager: tried to apply null upgrade");
            return;
        }

        switch (upgrade.type)
        {
            case UpgradeType.FireRate:
                ApplyFireRateUpgrade(upgrade.value);
                break;

            case UpgradeType.IceRate:
                ApplyIceRateUpgrade(upgrade.value);
                break;

            case UpgradeType.LightningRate:
                ApplyLightningRateUpgrade(upgrade.value);
                break;

            case UpgradeType.Heal:
                ApplyHealUpgrade(upgrade.value);
                break;

            default:
                Debug.LogWarning("upgrade manager: unhandled type " + upgrade.type);
                break;
        }

        Debug.Log("applied upgrade: " + upgrade.upgradeName);
    }

    // fire rate upgrade
    void ApplyFireRateUpgrade(float value)
    {
        fireRateMultiplier *= (1f + value);
    }

    // ice rate upgrade
    void ApplyIceRateUpgrade(float value)
    {
        iceRateMultiplier *= (1f + value);
    }

    // lightning rate / cooldown upgrade
    void ApplyLightningRateUpgrade(float value)
    {
        lightningRateMultiplier *= (1f + value);
    }

    // heal upgrade
    void ApplyHealUpgrade(float hearts)
    {
        if (player == null)
        {
            Debug.LogWarning("upgrade manager: no player hooked up");
            return;
        }

        player.Heal(hearts);
    }
}
