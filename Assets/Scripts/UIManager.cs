using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI runeText;
    public Slider healthBar;
    public Image damageFlash;         // red overlay image

    private float elapsedTime = 0f;
    private float smoothSpeed = 10f;
    private float targetHealth;
    private float flashAlpha = 0f;    // current flash opacity
    private float flashFadeSpeed = 2f; // how fast it fades out

    void Start()
    {
        if (healthBar != null)
            targetHealth = healthBar.value;
    }

    void Update()
    {
        // timer text
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        // smooth hp bar
        if (healthBar != null)
            healthBar.value = Mathf.Lerp(healthBar.value, targetHealth, Time.deltaTime * smoothSpeed);

        // fade flash out over time
        if (damageFlash != null && flashAlpha > 0f)
        {
            flashAlpha -= Time.deltaTime * flashFadeSpeed;
            Color c = damageFlash.color;
            c.a = flashAlpha;
            damageFlash.color = c;
        }
    }

    public void UpdateRuneUI(int count)
    {
        runeText.text = "Runes: " + count + "/3";
    }

    public void UpdateHealth(float newValue)
    {
        targetHealth = newValue;
        healthBar.value = newValue;
    }
    public void TriggerDamageFlash()
    {
        if (damageFlash == null) return;

        flashAlpha = 0.5f; // how bright the flash is
        Color c = damageFlash.color;
        c.a = flashAlpha;
        damageFlash.color = c;
    }
}
