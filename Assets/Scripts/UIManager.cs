using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI fireRuneText;
    public TextMeshProUGUI iceRuneText;
    public TextMeshProUGUI lightningRuneText;
    public Slider healthBar;
    public Image damageFlash;         // red overlay image

    private float elapsedTime = 0f;
    private float smoothSpeed = 10f;
    private float targetHealth;
    private float flashAlpha = 0f;    // current flash opacity
    private float flashFadeSpeed = 2f; // how fast it fades out

    public GameObject gameOverPanel;
    public TextMeshProUGUI survivalTimeText;

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

    public void UpdateRuneUI(int fireCount, int iceCount, int lightningCount)
    {
        Color normalColor = Color.white;
        Color readyColor = Color.green; // when ready, text becomes green

        fireRuneText.text = $"Fire Runes: {fireCount}/5";
        fireRuneText.color = (fireCount >= 5) ? readyColor : normalColor;

        iceRuneText.text = $"Ice Runes: {iceCount}/5";
        iceRuneText.color = (iceCount >= 5) ? readyColor : normalColor;

        lightningRuneText.text = $"Lightning Runes: {lightningCount}/5";
        lightningRuneText.color = (lightningCount >= 5) ? readyColor : normalColor;
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

    public void ShowGameOverPanel()
    {
        if (gameOverPanel == null) return;

        // calculate survival time before showing
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (survivalTimeText != null)
            survivalTimeText.text = $"You survived {minutes:00}:{seconds:00}";

        gameOverPanel.SetActive(true);
    }


}
