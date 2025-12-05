using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("button sfx")]
    public AudioSource audioSource;
    public AudioClip buttonClick;
    public float clickVolume = 1f;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnPlayClicked()
    {
        PlayClick();
        SceneManager.LoadScene("MainScene"); 
    }

    public void OnExitClicked()
    {
        PlayClick();

        Application.Quit();

#if UNITY_EDITOR
        // so Exit works in editor too
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void PlayClick()
    {
        if (audioSource != null && buttonClick != null)
            audioSource.PlayOneShot(buttonClick, clickVolume);
    }
}
