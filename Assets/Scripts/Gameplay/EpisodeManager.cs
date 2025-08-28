using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class EpisodeManager : MonoBehaviour
{
    public Episode currentEpisode;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
    public VideoPlayer videoPlayer;
    public Camera mainCamera;
    public Animator hostAnimator;
    public List<Animator> audienceAnimators;
    public Animator guestAnimator;
    public GameObject guestObject;
    public Animator curtainAnimator; // Added curtain animator
    public GameObject curtainObject;
    public Vector3 curtainRaisedPosition;

    private int dialogueIndex = 0;

    public void PlayEpisode()
    {
        if (guestObject != null)
            guestObject.SetActive(false); // Hide guest at start
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        float introDuration = 8f;
        if (currentEpisode.intro.introSong)
            introDuration = currentEpisode.intro.introSong.length;

        // Play intro song
        if (currentEpisode.intro.introSong)
        {
            musicSource.clip = currentEpisode.intro.introSong;
            musicSource.Play();
        }
        // Play intro video
        if (currentEpisode.intro.introVideo)
        {
            videoPlayer.clip = currentEpisode.intro.introVideo;
            videoPlayer.Play();
        }
        // Play voice over
        if (currentEpisode.intro.introVoiceOver)
        {
            voiceSource.clip = currentEpisode.intro.introVoiceOver;
            voiceSource.Play();
        }
        // Play audience cheer
        if (currentEpisode.intro.audienceCheerAndClap)
        {
            sfxSource.clip = currentEpisode.intro.audienceCheerAndClap;
            sfxSource.Play();
        }

        // Wait for 6 seconds, then start fade and camera pan
        yield return new WaitForSeconds(6f);
        float panDuration = introDuration - 6f;
        // Start video fade out (implement your own fade logic here)
        StartCoroutine(FadeOutVideo(videoPlayer, panDuration)); // Fades out over remaining duration
        // Start camera pan (over the remaining duration)
        yield return StartCoroutine(PanCamera(panDuration));

        // Snap to last pan area end position/rotation
        int panCount = currentEpisode.intro.cameraPanTargets.Count;
        if (panCount > 0)
        {
            var lastArea = currentEpisode.intro.cameraPanTargets[panCount - 1];
            if (lastArea != null)
            {
                mainCamera.transform.position = lastArea.endPosition;
                mainCamera.transform.rotation = Quaternion.Euler(lastArea.endRotation);
            }
        }
        // Play audience clap (same audio file)
        if (currentEpisode.intro.audienceCheerAndClap)
        {
            sfxSource.clip = currentEpisode.intro.audienceCheerAndClap;
            sfxSource.Play();
        }
        // Start dialogue
        PlayDialogueStep(0);
    }

    private IEnumerator PanCamera(float panDuration)
    {
        int panCount = currentEpisode.intro.cameraPanTargets.Count;
        if (panCount == 0) yield break;
        float positionThreshold = 0.01f;
        float rotationThreshold = 0.5f;
        float panSpeed = 2f; // You can expose this as a public variable
        foreach (var panArea in currentEpisode.intro.cameraPanTargets)
        {
            mainCamera.transform.position = panArea.startPosition;
            mainCamera.transform.rotation = Quaternion.Euler(panArea.startRotation);
            Vector3 targetPos = panArea.endPosition;
            Quaternion targetRot = Quaternion.Euler(panArea.endRotation);
            while (Vector3.Distance(mainCamera.transform.position, targetPos) > positionThreshold ||
                   Quaternion.Angle(mainCamera.transform.rotation, targetRot) > rotationThreshold)
            {
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPos, panSpeed * Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, targetRot, panSpeed * Time.deltaTime * 100f);
                yield return null;
            }
            // Snap to end position/rotation for this area
            mainCamera.transform.position = targetPos;
            mainCamera.transform.rotation = targetRot;
        }
    }

    // Dummy fade coroutine (replace with your own video fade logic)
    private IEnumerator FadeOutVideo(VideoPlayer vp, float fadeDuration)
    {
        // Example: fade out video alpha (requires custom implementation)
        // for (float t = 0; t < fadeDuration; t += Time.deltaTime) { ... }
        yield return new WaitForSeconds(fadeDuration);
    }

    public void PlayDialogueStep(int index)
    {
        if (index < 0 || index >= currentEpisode.dialogueSteps.Count) return;
        dialogueIndex = index;
        var step = currentEpisode.dialogueSteps[index];
        // Show guest if this step triggers it
        if (step.showGuest && guestObject != null && !guestObject.activeSelf)
            guestObject.SetActive(true);
        // Raise curtain by moving it if needed
        if (step.raiseCurtain && curtainObject != null)
            curtainObject.transform.position = curtainRaisedPosition;
        // Play voice
        if (step.voiceClip)
        {
            voiceSource.clip = step.voiceClip;
            voiceSource.Play();
        }
        // Move camera
        if (step.cameraTarget)
        {
            mainCamera.transform.position = step.cameraTarget.position;
            mainCamera.transform.rotation = step.cameraTarget.rotation;
        }
        // Play animations
        if (!string.IsNullOrEmpty(step.hostAnimationState))
            hostAnimator.SetTrigger(step.hostAnimationState);
        if (!string.IsNullOrEmpty(step.audienceAnimationState) && audienceAnimators != null)
            foreach (var anim in audienceAnimators)
                if (anim != null)
                    anim.SetTrigger(step.audienceAnimationState);
        if (!string.IsNullOrEmpty(step.guestAnimationState))
            guestAnimator.SetTrigger(step.guestAnimationState);
        // Show dialogue text (implement your own UI logic)
        // ...
    }

    public void NextDialogueStep()
    {
        PlayDialogueStep(dialogueIndex + 1);
    }
}
