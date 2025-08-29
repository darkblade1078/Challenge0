using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CameraArea
{
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue State")]
    public string[] currentDialogue;
    public string[] currentSpeaker;
    public AudioClip[] currentVoiceClip;
    public CameraArea[] currentCameraTarget;
    public bool[] isResponseExpected;
    public string[][] responses;

    [Header("Audio")]
    public AudioSource HostAudioSource;
    public AudioSource GuestAudioSource;
    public AudioSource AudienceAudioSource;

    public AudioClip[] VoiceClips;

    [Header("Animators")]
    public Animator hostAnimator;
    public Animator guestAnimator;
    public List<Animator> audienceAnimators;

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public GameObject responseUI;

    [Header("Camera")]
    public Camera cam;

    public IEnumerator StartDialogue()
    {
        int dialogueIndex = 0;
        while (dialogueIndex < currentDialogue.Length)
        {
            // Set camera position and rotation for this dialogue step
            if (currentCameraTarget != null && dialogueIndex < currentCameraTarget.Length)
            {
                cam.transform.position = currentCameraTarget[dialogueIndex].cameraPosition;
                cam.transform.rotation = Quaternion.Euler(currentCameraTarget[dialogueIndex].cameraRotation);
            }

            // Update dialogue text UI
            if (dialogueText != null)
                dialogueText.text = currentDialogue[dialogueIndex];

            // TODO: Add logic to handle displaying dialogue and shit
            Debug.Log($"Dialogue: {currentDialogue[dialogueIndex]}");

            // Play voice clip if available or wait 2 seconds (The 2 seconds is a fallback if no voice clip)
            float waitTime = 2f;
            if (currentVoiceClip != null && dialogueIndex < currentVoiceClip.Length && currentVoiceClip[dialogueIndex] != null)
            {
                if (dialogueIndex < VoiceClips.Length && VoiceClips[dialogueIndex] != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource == null)
                        audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = VoiceClips[dialogueIndex];
                    audioSource.Play();
                    waitTime = VoiceClips[dialogueIndex].length;
                }
            }

            // Wait for voice clip to finish or 2 seconds if none
            float timer = 0f;
            bool canAdvance = false;

            while (!canAdvance)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                    canAdvance = true;
                if (canAdvance && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    break;
                yield return null;
            }

            // Stop audio if still playing (Not needed but it's good to have)
            AudioSource src = GetComponent<AudioSource>();
            if (src != null && src.isPlaying)
                src.Stop();
            dialogueIndex++;
        }
    }
}