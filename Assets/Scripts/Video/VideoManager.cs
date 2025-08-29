using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoManager : MonoBehaviour
{

    public IEnumerator PlayVideo(VideoPlayer videoPlayer, RawImage videoImage, VideoClip clip, float fadeStart = 6f, float fadeDuration = 2f)
    {
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.clip = clip;
        videoPlayer.Play();

        // Wait until video actually starts playing
        yield return new WaitUntil(() => videoPlayer.isPlaying);

        // Wait until fade should start
        float elapsed = 0f;
        while (elapsed < fadeStart && videoPlayer.isPlaying)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out video
        float fadeElapsed = 0f;
        Color startColor = videoImage.color;
        while (fadeElapsed < fadeDuration && videoPlayer.isPlaying)
        {
            float t = fadeElapsed / fadeDuration;
            videoImage.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            fadeElapsed += Time.deltaTime;
            yield return null;
        }
        videoImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f); // Ensure fully transparent

        // Wait until video finishes
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
    }
}