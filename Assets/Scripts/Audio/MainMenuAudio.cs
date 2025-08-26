using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public AudioClip musicClip;

    private void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

}
