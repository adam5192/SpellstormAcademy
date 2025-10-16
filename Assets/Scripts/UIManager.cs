using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI runeText;
    public Slider healthBar;

    private float elapsedTime = 0f;

    void Update()
    {
        // update timer every frame
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateRuneUI(int count)
    {
        runeText.text = "Runes: " + count + "/3";
    }

    public void UpdateHealth(float value)
    {
        healthBar.value = value;
    }
}
