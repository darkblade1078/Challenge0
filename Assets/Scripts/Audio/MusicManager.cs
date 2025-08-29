using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public IEnumerator PlaySong(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }
}