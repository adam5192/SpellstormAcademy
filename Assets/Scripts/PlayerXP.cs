using UnityEngine;

// handles xp + level ups
public class PlayerXP : MonoBehaviour
{
    [Header("xp / level")]
    public int currentLevel = 1;      // starting level
    public int currentXP = 0;         // current xp towards next level
    public int xpToNextLevel = 0;     // needed for next level

    public int baseXP = 5;            // base xp at level 1
    public int xpPerLevel = 3;        // extra xp per level (vs style)

    [Header("ui")]
    public UpgradeMenu upgradeMenu;  

    void Start()
    {
        // setup first xp target
        xpToNextLevel = ComputeXPForLevel(currentLevel);
    }

    // call this when player picks up a rune
    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;

        // in case gain more than one level at once
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    // simple xp curve: xp(n) = base + n * step
    int ComputeXPForLevel(int level)
    {
        return baseXP + (level * xpPerLevel);
    }

    // handle level up and open menu
    void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = ComputeXPForLevel(currentLevel);

        Debug.Log("leveled up to " + currentLevel);

        if (upgradeMenu != null)
        {
            upgradeMenu.OpenLevelUpMenu();
        }
        else
        {
            Debug.LogWarning("player xp: no upgrade menu hooked up");
        }
    }
}
