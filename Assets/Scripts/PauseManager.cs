using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("ui")]
    public GameObject pausePanel;  

    private bool isPaused = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        // ESC toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // dont allow pausing if the game is already over
        if (GameManager.instance != null && GameManager.instance.IsGameOver)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        // make sure game is unfrozen in the menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
