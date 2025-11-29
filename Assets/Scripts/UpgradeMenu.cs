using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// handles pause + upgrade choice ui
public class UpgradeMenu : MonoBehaviour
{
    [Header("panel")]
    public GameObject panelRoot;  // main panel object

    [System.Serializable]
    public class UpgradeButtonUI
    {
        public Button button;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;

        [HideInInspector] public UpgradeData assignedUpgrade; // set at runtime
    }

    [Header("buttons")]
    public List<UpgradeButtonUI> upgradeButtons = new List<UpgradeButtonUI>(); // usually 3

    [Header("upgrade pool")]
    public List<UpgradeData> allUpgrades = new List<UpgradeData>(); // all possible upgrades

    [Header("refs")]
    public UpgradeManager upgradeManager; 

    bool isOpen = false;
    float previousTimeScale = 1f;

    void Start()
    {
        // make sure panel is hidden at start
        if (panelRoot != null)
            panelRoot.SetActive(false);

        // hook buttons
        foreach (var ub in upgradeButtons)
        {
            if (ub != null && ub.button != null)
            {
                // capture local ref for closure
                UpgradeButtonUI localUB = ub;
                ub.button.onClick.AddListener(() => OnUpgradeButtonClicked(localUB));
            }
        }
    }

    // called from player xp when we level up
    public void OpenLevelUpMenu()
    {
        if (allUpgrades.Count == 0)
        {
            Debug.LogWarning("upgrade menu: no upgrades in pool");
            return;
        }

        if (isOpen) return;
        isOpen = true;

        // pause game
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // pick some random upgrades
        List<UpgradeData> options = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            var ub = upgradeButtons[i];

            if (i < options.Count)
            {
                var upgrade = options[i];
                ub.assignedUpgrade = upgrade;

                if (ub.titleText != null)
                    ub.titleText.text = upgrade.upgradeName;

                if (ub.descriptionText != null)
                    ub.descriptionText.text = upgrade.description;

                if (ub.button != null)
                    ub.button.interactable = true;
            }
            else
            {
                ub.assignedUpgrade = null;

                if (ub.titleText != null)
                    ub.titleText.text = "";

                if (ub.descriptionText != null)
                    ub.descriptionText.text = "";

                if (ub.button != null)
                    ub.button.interactable = false;
            }
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    // close ui + unpause
    void CloseLevelUpMenu()
    {
        if (!isOpen) return;
        isOpen = false;

        if (panelRoot != null)
            panelRoot.SetActive(false);

        Time.timeScale = previousTimeScale;
    }

    // when player presses one of the upgrade buttons
    void OnUpgradeButtonClicked(UpgradeButtonUI buttonUI)
    {
        if (buttonUI == null || buttonUI.assignedUpgrade == null)
        {
            Debug.LogWarning("upgrade menu: clicked button with no upgrade");
            return;
        }

        if (upgradeManager != null)
        {
            upgradeManager.ApplyUpgrade(buttonUI.assignedUpgrade);
        }
        else
        {
            Debug.LogWarning("upgrade menu: no upgrade manager hooked up");
        }

        CloseLevelUpMenu();
    }

    // get up to "count" random upgrades, no duplicates in this roll
    List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> chosen = new List<UpgradeData>();
        List<UpgradeData> poolCopy = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count; i++)
        {
            if (poolCopy.Count == 0)
                break;

            UpgradeData picked = GetWeightedRandomUpgrade(poolCopy);
            if (picked != null)
            {
                chosen.Add(picked);
                poolCopy.Remove(picked); // avoid same upgrade twice in one level up
            }
        }

        return chosen;
    }

    // pick one upgrade using weight
    UpgradeData GetWeightedRandomUpgrade(List<UpgradeData> list)
    {
        if (list == null || list.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var u in list)
        {
            totalWeight += u.GetWeight();
        }

        float r = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var u in list)
        {
            cumulative += u.GetWeight();
            if (r <= cumulative)
                return u;
        }

        // fallback just in case
        return list[list.Count - 1];
    }
}
