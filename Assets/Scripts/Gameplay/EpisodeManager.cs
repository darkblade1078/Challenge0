using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[System.Serializable]
public class CameraPanArea
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 startRotation;
    public Vector3 endRotation;
}

public class EpisodeManager : MonoBehaviour
{
    [Header("Camera")]
    public float camSpeed;
    public Camera cam;
    public List<CameraPanArea> panAreas;

    [Header("Video")]
    public float videoFadeStart = 8f;
    public float videoFadeDuration = 2f;
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public VideoClip introVideo;
    public GameObject videoRenderer;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip introMusic;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;
    public GameObject dialogueUI;

    void Start()
    {
        if (videoRenderer != null)
            videoRenderer.SetActive(true);

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {

        //TODO: get everyone to clap and cheer

        // Set the guest to the starting position and rotation
        if (dialogueManager != null && dialogueManager.spawnedGuest != null && dialogueManager.guestSpawnLocation != null)
        {
            if (dialogueManager.spawnedGuest != null)
            {
                dialogueManager.spawnedGuest.transform.position = dialogueManager.guestSpawnLocation;
                dialogueManager.spawnedGuest.transform.rotation = Quaternion.Euler(dialogueManager.guestSpawnRotation);
            }
            else
            {
                GameObject guest = Instantiate(dialogueManager.spawnedGuest, dialogueManager.guestSpawnLocation, Quaternion.Euler(dialogueManager.guestSpawnRotation));
                dialogueManager.guestAnimator = guest.GetComponent<Animator>();
            }
        }

        // Play intro music
        MusicManager musicManager = new MusicManager();
        Coroutine musicCoroutine = StartCoroutine(musicManager.PlaySong(musicSource, introMusic));

        // Play video
        VideoManager videoManager = new VideoManager();
        StartCoroutine(videoManager.PlayVideo(videoPlayer, videoImage, introVideo, videoFadeStart, videoFadeDuration));

        // Wait until panning should start
        yield return new WaitForSeconds(videoFadeStart);

        // Start camera panning
        EpisodePanning panner = new EpisodePanning(cam);
        yield return StartCoroutine(panner.Pan(panAreas, camSpeed));

        // Wait for music to finish
        if (musicCoroutine != null)
            yield return musicCoroutine;

        // Start DialogueManager
        if (dialogueManager != null)
        {
            if (videoRenderer != null)
                videoRenderer.SetActive(false);

            if (dialogueUI != null)
                dialogueUI.SetActive(true);

            // Assuming DialogueManager has a StartDialogue coroutine
            yield return StartCoroutine(dialogueManager.StartDialogue());
        }
    }
}