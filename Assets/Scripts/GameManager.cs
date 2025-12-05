using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    [Header("game over sfx")]
    public AudioClip gameOverSfx;
    public float gameOverVolume = 1f;
    private AudioSource sfxSource;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.ignoreListenerPause = true;  
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;


        // make sure in a running state
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (CameraShake.instance != null)
            CameraShake.instance.allowShake = true;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // play game over sound before pausing global audio
        if (gameOverSfx != null && sfxSource != null)
            sfxSource.PlayOneShot(gameOverSfx, gameOverVolume);

        // freeze the world
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (HitPause.instance != null)
            HitPause.instance.CancelOnGameOver();

        if (CameraShake.instance != null)
            CameraShake.instance.allowShake = false;

        foreach (Animator anim in FindObjectsOfType<Animator>())
            anim.speed = 0f;

        UIManager ui = UIManager.instance != null
            ? UIManager.instance
            : FindObjectOfType<UIManager>();

        if (ui != null)
            ui.ShowGameOverPanel();
    }


    public void RestartGame()
    {
        // undo global freezes before reload (just in case)
        AudioListener.pause = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // unfreeze everything before switching scenes
        AudioListener.pause = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

}
