using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class EpisodeManager : MonoBehaviour
{
    public float cameraPanSpeed = 2f;
    public float cameraPositionThreshold = 0.01f;
    public float cameraRotationThreshold = 0.5f;
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
    public List<CameraPan> cameraPans;

    [System.Serializable]
    public class CameraPan {
        public Vector3 startingCameraPosition;
        public Vector3 startingCameraRotation;
        public Vector3 targetCameraPosition;
        public Vector3 targetCameraRotation;
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(StartCameraPanning());
    }

    private IEnumerator StartCameraPanning()
    {
        if (cameraPans == null || cameraPans.Count == 0)
            yield break;

        for(int i = 0; i < cameraPans.Count; i++)
        {
            var panArea = cameraPans[i];
            mainCamera.transform.position = panArea.startingCameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(panArea.startingCameraRotation);

            bool atPosition = Vector3.Distance(mainCamera.transform.position, panArea.targetCameraPosition) < cameraPositionThreshold;
            bool atRotation = Quaternion.Angle(mainCamera.transform.rotation, Quaternion.Euler(panArea.targetCameraRotation)) < cameraRotationThreshold;

            while()
        }
    }
}
