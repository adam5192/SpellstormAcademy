using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("xp / level")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 0;

    [Header("xp curve")]
    public int baseXP = 5;      // xp needed for level 1 -> 2
    public int xpPerLevel = 3;  // extra xp needed each level

    [Header("upgrade menu")]
    public UpgradeMenu upgradeMenu; // hooked from inspector

    private UIManager ui;

    void Start()
    {
        ui = UIManager.instance; 

        xpToNextLevel = ComputeXPForLevel(currentLevel);
        UpdateXPUI();
    }


    // external call when player gets xp
    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;

        // handle multiple level ups if you gain a bunch at once
        bool leveledUp = false;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
            leveledUp = true;
        }

        if (!leveledUp)
            UpdateXPUI();
    }

    int ComputeXPForLevel(int level)
    {
        // simple linear growth like vampire survivors (ish)
        return baseXP + level * xpPerLevel;
    }

    void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = ComputeXPForLevel(currentLevel);

        // open level up menu
        if (upgradeMenu != null)
            upgradeMenu.OpenLevelUpMenu();

        // update ui with new level + new bar max
        UpdateXPUI();
    }

    void UpdateXPUI()
    {
        if (ui != null)
            ui.UpdateXPBar(currentLevel, currentXP, xpToNextLevel);
    }
}
