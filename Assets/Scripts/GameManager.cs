using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f; // make sure game runs at normal speed
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER");

        // stop all gameplay
        Time.timeScale = 0f;

        // show game over UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
            ui.ShowGameOverPanel();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
