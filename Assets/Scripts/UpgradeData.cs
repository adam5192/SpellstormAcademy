using UnityEngine;

// basic data for a single upgrade
[CreateAssetMenu(menuName = "spellstorm/upgrade", fileName = "new_upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("info")]
    public string upgradeName;
    [TextArea] public string description;

    [Header("type + value")]
    public UpgradeType type;

    // for rate upgrades: 0.15f = +15%
    // for heal: how many hearts
    public float value = 0.15f;

    [Header("rarity / weight")]
    public UpgradeRarity rarity = UpgradeRarity.Common;

    // if > 0, use this weight instead of default
    public float manualWeightOverride = 0f;

    // get weight for random roll
    public float GetWeight()
    {
        if (manualWeightOverride > 0f)
            return manualWeightOverride;

        switch (rarity)
        {
            case UpgradeRarity.Common: return 80f;
            case UpgradeRarity.Uncommon: return 15f;
            case UpgradeRarity.Rare: return 5f;
            default: return 1f;
        }
    }
}
