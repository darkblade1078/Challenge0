using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

[System.Serializable]
public class DialogueStep {
    public string dialogueText;
    public AudioClip voiceClip;
    public Transform cameraTarget; // Where the camera should move/look
    public string hostAnimationState; // Animator state or trigger name
    public string audienceAnimationState;
    public string guestAnimationState;
    public bool showGuest; // Trigger guest appearance
}

[System.Serializable]
public class EpisodeIntro {
    public AudioClip introSong;
    public VideoClip introVideo;
    public AudioClip introVoiceOver;
    public AudioClip audienceCheerAndClap;
}

[CreateAssetMenu(fileName = "Episode", menuName = "TalkShow/Episode", order = 1)]
public class Episode : ScriptableObject {
    public EpisodeIntro intro;
    public List<DialogueStep> dialogueSteps;
}
