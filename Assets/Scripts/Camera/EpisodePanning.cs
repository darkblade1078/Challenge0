using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodePanning : MonoBehaviour
{
    public float positionThreshold = 0.01f;
    public float rotationThreshold = 0.5f;

    private Camera cam;

    public EpisodePanning(Camera cam)
    {
        this.cam = cam;
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public IEnumerator Pan(List<CameraPanArea> panAreas, float speed)
    {
        for (int i = 0; i < panAreas.Count; i++)
        {
            cam.transform.position = panAreas[i].startPosition;
            cam.transform.rotation = Quaternion.Euler(panAreas[i].startRotation);
            bool atPosition = false;
            bool atRotation = false;
            while (!atPosition || !atRotation)
            {
                float positionDist = Vector3.Distance(cam.transform.position, panAreas[i].endPosition);
                float rotationDist = Quaternion.Angle(cam.transform.rotation, Quaternion.Euler(panAreas[i].endRotation));
                atPosition = positionDist < positionThreshold;
                atRotation = rotationDist < rotationThreshold;

                float moveSpeed = speed;
                float rotateSpeed = speed * 10;

                // If final pan area, ease out speed as we approach the end
                if (i == panAreas.Count - 1)
                {
                    float easeT = Mathf.Clamp01(1f - (positionDist / 2f)); // 2 units is arbitrary, adjust for your scene
                    moveSpeed = Mathf.Lerp(speed, speed * 0.25f, easeT); // Slow to 25% speed near end
                    rotateSpeed = Mathf.Lerp(speed * 10, speed * 2.5f, easeT);
                }

                if (!atPosition)
                    cam.transform.position = Vector3.MoveTowards(cam.transform.position, panAreas[i].endPosition, moveSpeed * Time.deltaTime);

                if (!atRotation)
                    cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.Euler(panAreas[i].endRotation), rotateSpeed * Time.deltaTime);
                    
                yield return null;
            }
        }
    }
}