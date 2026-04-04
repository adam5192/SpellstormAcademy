using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("button sfx")]
    public AudioSource audioSource;
    public AudioClip buttonClick;
    public float clickVolume = 1f;

    [Header("play narration")]
    public AudioClip playNarration;
    public float narrationVolume = 1f;

    private bool isLoading = false;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public void OnPlayClicked()
    {
        if (isLoading) return;
        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        isLoading = true;

        if (audioSource != null)
        {
            if (buttonClick != null)
                audioSource.PlayOneShot(buttonClick, clickVolume);

            if (playNarration != null)
            {
                yield return new WaitForSeconds(buttonClick != null ? buttonClick.length : 0f);
                audioSource.clip = playNarration;
                audioSource.volume = narrationVolume;
                audioSource.Play();

                yield return new WaitForSeconds(playNarration.length);
            }
        }

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