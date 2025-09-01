using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class FollowUpStep
{
    public List<string> responses;
    public string speaker;
    public List<AudioClip> voiceClips;
    public CameraArea cameraArea;
}

[System.Serializable]
public class ResponseStep
{
    public string speaker;
    public List<string> responses;
    public List<bool> isBadResponse;
    public List<AudioClip> voiceClips;
    public FollowUpStep followUps;
    public CameraArea cameraArea;
}

[System.Serializable]
public class CameraArea
{
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
}

[System.Serializable]
public class DialogueStep
{
    public string dialogue;
    public string speaker;
    public AudioClip voiceClip;
    public AudioClip musicClip;
    public CameraArea cameraArea;
    public Sprite image;
    public bool showDialogueBox = true;
    public ResponseStep response;
    public bool spawnGuest = false; // Should guest spawn in this step?
    public string hostAnimation;
    public string guestAnimation;
    public string audienceAnimation;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue State")]
    public DialogueStep[] dialogueSteps;

    [Header("Audio")]
    public AudioClip endMusicClip;

    [Header("Animators")]
    public Animator hostAnimator;
    public Animator guestAnimator;
    public List<Animator> audienceAnimators;

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public GameObject responseUI;
    public RawImage dialogueBoxImage;

    [Header("Camera")]
    public Camera cam;
    public Vector3 camEndLocation;
    public Vector3 camEndRotation;

    [Header("Guest Model")]
    public GameObject spawnedGuest;
    public Vector3 guestSpawnLocation;
    public Vector3 guestSpawnRotation;
    public Vector3 guestEndLocation;
    public Vector3 guestEndRotation;

    public IEnumerator StartDialogue()
    {
        int dialogueIndex = 0;
        while (dialogueIndex < dialogueSteps.Length)
        {
            // Play host animation if specified
            if (hostAnimator != null && !string.IsNullOrEmpty(dialogueSteps[dialogueIndex].hostAnimation))
                hostAnimator.Play(dialogueSteps[dialogueIndex].hostAnimation);

            // Play guest animation if specified
            if (guestAnimator != null && !string.IsNullOrEmpty(dialogueSteps[dialogueIndex].guestAnimation))
                guestAnimator.Play(dialogueSteps[dialogueIndex].guestAnimation);

            // Play audience animation if specified
            if (audienceAnimators != null && !string.IsNullOrEmpty(dialogueSteps[dialogueIndex].audienceAnimation))
                foreach (var anim in audienceAnimators)
                    if (anim != null)
                        anim.Play(dialogueSteps[dialogueIndex].audienceAnimation);

            // Set camera position and rotation for this dialogue step
            if (dialogueSteps[dialogueIndex].cameraArea != null)
            {
                cam.transform.position = dialogueSteps[dialogueIndex].cameraArea.cameraPosition;
                cam.transform.rotation = Quaternion.Euler(dialogueSteps[dialogueIndex].cameraArea.cameraRotation);
            }

            // Update dialogue text UI
            if (dialogueText != null)
                dialogueText.text = dialogueSteps[dialogueIndex].dialogue;

            // Update speaker text UI
            if (speakerText != null)
                speakerText.text = dialogueSteps[dialogueIndex].speaker;

            if (dialogueSteps[dialogueIndex].showDialogueBox)
                dialogueText.transform.parent.gameObject.SetActive(true);
            else
                dialogueText.transform.parent.gameObject.SetActive(false);

            // Fade in image if present
            if (dialogueSteps[dialogueIndex].image != null && dialogueBoxImage != null)
            {
                dialogueBoxImage.texture = null;
                dialogueBoxImage.gameObject.SetActive(true);
                dialogueBoxImage.color = new Color(1f, 1f, 1f, 0f);
                // Convert Sprite to Texture2D for RawImage
                Sprite sprite = dialogueSteps[dialogueIndex].image;
                if (sprite.texture != null)
                    dialogueBoxImage.texture = sprite.texture;
                // Fade in
                float fadeTime = 0.5f;
                float fadeElapsed = 0f;
                while (fadeElapsed < fadeTime)
                {
                    fadeElapsed += Time.deltaTime;
                    dialogueBoxImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, fadeElapsed / fadeTime));
                    yield return null;
                }
                dialogueBoxImage.color = new Color(1f, 1f, 1f, 1f);
            }

            // Play music if available and wait for it to finish before allowing advance
            float musicWaitTime = 0f;
            if (dialogueSteps[dialogueIndex].musicClip != null)
            {
                AudioSource musicSource = GetComponent<AudioSource>();
                if (musicSource == null)
                    musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.clip = dialogueSteps[dialogueIndex].musicClip;
                musicSource.volume = 0.6f; // Lower volume by 40%
                musicSource.Play();
                musicWaitTime = dialogueSteps[dialogueIndex].musicClip.length;
            }

            // Spawn guest if required in this step
            if (dialogueSteps[dialogueIndex].spawnGuest && spawnedGuest != null)
            {
                Animator guestAnim = spawnedGuest.GetComponent<Animator>();
                if (guestAnim != null)
                    guestAnim.Play("Walk");

                // Move guest from current position to guestSpawnLocation over music length or 6 seconds
                Vector3 startPos = guestSpawnLocation;
                Vector3 endPos = new Vector3(5.37599993f, 0.463325977f, -3.77999997f);
                Quaternion startRot = spawnedGuest.transform.rotation;
                Quaternion endRot = Quaternion.Euler(guestSpawnRotation);
                float moveTime = musicWaitTime > 0f ? musicWaitTime : 6f;
                float elapsed = 0f;
                Vector3 camOffset = new Vector3(0, 1.5f, 2.5f); // Camera stays in front of guest
                while (elapsed < moveTime)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / moveTime);
                    spawnedGuest.transform.position = Vector3.Lerp(startPos, endPos, t);
                    spawnedGuest.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

                    // Camera stays in front of guest
                    if (cam != null)
                    {
                        Vector3 forward = spawnedGuest.transform.forward;
                        cam.transform.position = spawnedGuest.transform.position + forward * camOffset.z + Vector3.up * camOffset.y;
                        cam.transform.LookAt(spawnedGuest.transform.position + Vector3.up * 1.2f);
                    }
                    yield return null;
                }

                dialogueIndex++; // Skip to next dialogue step after spawning
                continue; // Skip the rest of this loop to avoid double increment
            }

            // Wait for music to finish
            float musicTimer = 0f;
            while (musicTimer < musicWaitTime)
            {
                musicTimer += Time.deltaTime;
                yield return null;
            }

            // Play voice clip if available or wait 2 seconds (fallback)
            float waitTime = 2f;
            if (dialogueSteps[dialogueIndex].voiceClip != null)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = dialogueSteps[dialogueIndex].voiceClip;
                audioSource.Play();
                waitTime = dialogueSteps[dialogueIndex].voiceClip.length;
            }

            // Wait for voice clip to finish or 2 seconds if none
            float timer = 0f;
            while (timer < waitTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // Response handling
            if (dialogueSteps[dialogueIndex].response != null && dialogueSteps[dialogueIndex].response.responses != null && dialogueSteps[dialogueIndex].response.responses.Count > 0)
            {
                // Camera jump for response step if specified
                if (dialogueSteps[dialogueIndex].response.cameraArea != null && cam != null)
                {
                    cam.transform.position = dialogueSteps[dialogueIndex].response.cameraArea.cameraPosition;
                    cam.transform.rotation = Quaternion.Euler(dialogueSteps[dialogueIndex].response.cameraArea.cameraRotation);
                }
                // Disable dialogue text while picking
                if (dialogueText != null)
                    dialogueText.gameObject.SetActive(false);

                // Set speaker name for response UI
                if (speakerText != null && !string.IsNullOrEmpty(dialogueSteps[dialogueIndex].response.speaker))
                    speakerText.text = dialogueSteps[dialogueIndex].response.speaker;

                // Activate response UI
                responseUI.SetActive(true);

                // Get all button children
                Button[] buttons = responseUI.GetComponentsInChildren<Button>(true);

                int responseCount = dialogueSteps[dialogueIndex].response.responses.Count;

                int chosenIndex = -1;

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (i < responseCount)
                    {
                        buttons[i].gameObject.SetActive(true);
                        TextMeshProUGUI txt = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                        if (txt != null)
                            txt.text = $"{i + 1}. {dialogueSteps[dialogueIndex].response.responses[i]}";
                        int idx = i;
                        buttons[i].onClick.RemoveAllListeners();
                        buttons[i].onClick.AddListener(() => { chosenIndex = idx; });
                    }
                    else
                    {
                        buttons[i].gameObject.SetActive(false);
                    }
                }

                // Wait for user to pick a response
                while (chosenIndex == -1)
                    yield return null;

                if (responseUI != null)
                    responseUI.SetActive(false);

                // Display chosen response as dialogue
                if (dialogueText != null)
                {
                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = dialogueSteps[dialogueIndex].response.responses[chosenIndex];
                }

                if (speakerText != null && !string.IsNullOrEmpty(dialogueSteps[dialogueIndex].response.speaker))
                    speakerText.text = dialogueSteps[dialogueIndex].response.speaker;

                dialogueText?.transform.parent.gameObject.SetActive(true);

                // Play the assigned response voice clip or wait 2 seconds if none
                float responseWaitTime = 2f;
                if (dialogueSteps[dialogueIndex].response.voiceClips != null && chosenIndex < dialogueSteps[dialogueIndex].response.voiceClips.Count && dialogueSteps[dialogueIndex].response.voiceClips[chosenIndex] != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource == null)
                        audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = dialogueSteps[dialogueIndex].response.voiceClips[chosenIndex];
                    audioSource.Play();
                    responseWaitTime = dialogueSteps[dialogueIndex].response.voiceClips[chosenIndex].length;
                }
                float responseTimer = 0f;
                while (responseTimer < responseWaitTime)
                {
                    responseTimer += Time.deltaTime;
                    yield return null;
                }
                // Wait for user input after response
                bool waitingForResponseAdvance = true;
                while (waitingForResponseAdvance)
                {
                    if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                        waitingForResponseAdvance = false;
                    yield return null;
                }
                // Handle follow-up logic
                FollowUpStep followUp = dialogueSteps[dialogueIndex].response.followUps;
                if (followUp != null)
                {
                    // Set speaker text to follow-up speaker if present
                    if (speakerText != null && !string.IsNullOrEmpty(followUp.speaker))
                        speakerText.text = followUp.speaker;
                    // Set dialogue text to follow-up response if present
                    if (dialogueText != null && followUp.responses != null && chosenIndex < followUp.responses.Count)
                        dialogueText.text = followUp.responses[chosenIndex];
                    // Camera jump for follow-up if specified
                    if (followUp.cameraArea != null && cam != null)
                    {
                        cam.transform.position = followUp.cameraArea.cameraPosition;
                        cam.transform.rotation = Quaternion.Euler(followUp.cameraArea.cameraRotation);
                    }
                    // Play follow-up voice clip or wait 2 seconds if none
                    float followWaitTime = 2f;
                    if (followUp.voiceClips != null && chosenIndex < followUp.voiceClips.Count && followUp.voiceClips[chosenIndex] != null)
                    {
                        AudioSource audioSource = GetComponent<AudioSource>();
                        if (audioSource == null)
                            audioSource = gameObject.AddComponent<AudioSource>();
                        audioSource.clip = followUp.voiceClips[chosenIndex];
                        audioSource.Play();
                        followWaitTime = followUp.voiceClips[chosenIndex].length;
                    }
                    float followTimer = 0f;
                    while (followTimer < followWaitTime)
                    {
                        followTimer += Time.deltaTime;
                        yield return null;
                    }
                    // Wait for user input after follow-up
                    bool waitingForFollowAdvance = true;
                    while (waitingForFollowAdvance)
                    {
                        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                            waitingForFollowAdvance = false;
                        yield return null;
                    }
                }
            }

            // Fade out and remove image if present
            if (dialogueSteps[dialogueIndex].image != null && dialogueBoxImage != null)
            {
                float fadeTime = 0.5f;
                float fadeElapsed = 0f;
                while (fadeElapsed < fadeTime)
                {
                    fadeElapsed += Time.deltaTime;
                    dialogueBoxImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, fadeElapsed / fadeTime));
                    yield return null;
                }
                dialogueBoxImage.color = new Color(1f, 1f, 1f, 0f);
                dialogueBoxImage.gameObject.SetActive(false);
                dialogueBoxImage.texture = null;
            }

            // Hide response UI
            responseUI.SetActive(false);

            // Wait for user input before advancing to next step
            bool waitingForAdvance = true;
            while (waitingForAdvance)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    waitingForAdvance = false;
                yield return null;
            }

            dialogueIndex++;
        }

        if (cam != null)
        {
            cam.transform.position = camEndLocation;
            cam.transform.rotation = Quaternion.Euler(camEndRotation);
        }

        //disable dialogue box at end
        if (dialogueText != null)
            dialogueText.transform.parent.gameObject.SetActive(false);

        // After all dialogue steps are finished, trigger end sequence
        // Set guest to ending position and rotation
        if (spawnedGuest != null)
        {
            spawnedGuest.transform.position = guestEndLocation;
            spawnedGuest.transform.rotation = Quaternion.Euler(guestEndRotation);
        }

        // Play end music
        float endMusicWaitTime = 0f;
        if (endMusicClip != null)
        {
            AudioSource musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
                musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = endMusicClip;
            musicSource.volume = 0.6f; // Lower volume by 40%
            musicSource.Play();
            endMusicWaitTime = endMusicClip.length;
        }

        // Host cheer
        if (hostAnimator != null)
            hostAnimator.Play("Cheer");

        // Audience cheer
        if (audienceAnimators != null)
            foreach (var anim in audienceAnimators)
                if (anim != null)
                    anim.Play("Cheer");

        // Guest dance
        if (spawnedGuest != null)
        {
            Animator guestAnim = spawnedGuest.GetComponent<Animator>();
            if (guestAnim != null)
                guestAnim.Play("Dance");
        }

        // Wait for end music to finish
        float endMusicTimer = 0f;
        while (endMusicTimer < endMusicWaitTime)
        {
            endMusicTimer += Time.deltaTime;
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}