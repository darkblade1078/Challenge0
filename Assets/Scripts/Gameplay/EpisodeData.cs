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
    public bool raiseCurtain; // Trigger curtain animation
}

[System.Serializable]
public class CameraPanArea {
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 startRotation;
    public Vector3 endRotation;
}

[System.Serializable]
public class EpisodeIntro {
    public AudioClip introSong;
    public VideoClip introVideo;
    public AudioClip introVoiceOver;
    public AudioClip audienceCheerAndClap;
    public List<CameraPanArea> cameraPanTargets; // At least 3 areas
}

[CreateAssetMenu(fileName = "Episode", menuName = "TalkShow/Episode", order = 1)]
public class Episode : ScriptableObject {
    public EpisodeIntro intro;
    public List<DialogueStep> dialogueSteps;
}
