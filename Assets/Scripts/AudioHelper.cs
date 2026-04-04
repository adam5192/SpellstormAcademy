using UnityEngine;

// small helper for one shot world sounds
public static class AudioHelper
{
    public static void PlayClipAtPointRandomized(AudioClip clip, Vector3 position, float volume, float minPitch, float maxPitch)
    {
        if (clip == null)
            return;

        GameObject tempAudio = new GameObject("temp_audio");
        tempAudio.transform.position = position;

        AudioSource src = tempAudio.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.pitch = Random.Range(minPitch, maxPitch);
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 20f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.Play();

        Object.Destroy(tempAudio, Mathf.Max(clip.length / Mathf.Max(0.01f, Mathf.Abs(src.pitch)), 0.1f));
    }
}
