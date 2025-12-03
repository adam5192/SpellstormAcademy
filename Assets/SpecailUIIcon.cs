using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialUIIcon : MonoBehaviour
{
    public Image icon;
    public Image fillOverlay;     // the "charging" overlay
    public TextMeshProUGUI countText;

    [Header("colors")]
    public Color readyColor = Color.white;
    public Color notReadyColor = new Color(1f, 1f, 1f, 0.4f);

    // call  every frame
    public void UpdateUI(bool ready, float progress01, int stackCount)
    {
        // icon brightness
        icon.color = ready ? readyColor : notReadyColor;

        // overlay fill (1 = empty, 0 = ready)
        fillOverlay.fillAmount = 1f - Mathf.Clamp01(progress01);

        // stack count
        countText.text = (stackCount > 0) ? stackCount.ToString() : "";
    }
}
