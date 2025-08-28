using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Video player
    public CanvasGroup canvasGroup; // Canvas we play the video on
    public float fadeDuration = 2f; // How long the fade lasts


    void Start()
    {
        videoPlayer.Play();
        StartCoroutine(CheckForFade());
    }

    System.Collections.IEnumerator CheckForFade()
    {
        // Wait until the video has a valid length
        while (videoPlayer.frameCount <= 0)
            yield return null;

        double videoLength = videoPlayer.length;

        // Start fade before video ends
        yield return new WaitForSeconds((float)(videoLength - fadeDuration));

        // Fade out
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
