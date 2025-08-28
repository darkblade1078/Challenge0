using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanning : MonoBehaviour
{
    public float speed = 2f;

    private int currentIndex = 0;

    private Camera cam;

    // Camera starting positions
    public List<Vector3> startingPositions;
    // Camera target positions to pan to
    public List<Vector3> targetPositions;
    // Camera Rotation look at positions
    public List<Vector3> lookAtPositions;
    // Camera Rotation target look at positions
    public List<Vector3> targetLookAt;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }


    void Start()
    {
        currentIndex = 0;
        cam.transform.position = startingPositions[currentIndex];
        cam.transform.rotation = Quaternion.Euler(lookAtPositions[currentIndex]);
    }

    void Update()
    {
        float positionThreshold = 0.01f;
        float rotationThreshold = 0.5f;

        // Check if the camera is close enough to the target position and rotation
        bool atPosition = Vector3.Distance(cam.transform.position, targetPositions[currentIndex]) < positionThreshold;
        bool atRotation = Quaternion.Angle(cam.transform.rotation, Quaternion.Euler(targetLookAt[currentIndex])) < rotationThreshold;

        if (!atPosition || !atRotation)
        {
            // Move the camera towards the target position
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, targetPositions[currentIndex], speed * Time.deltaTime);
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.Euler(targetLookAt[currentIndex]), speed * Time.deltaTime * 10);
        }
        else
        {
            currentIndex = (currentIndex + 1) % targetPositions.Count; // Reset to 0 after reaching the end
            // Set the camera to the new starting position and look at the new target
            cam.transform.position = startingPositions[currentIndex];
            cam.transform.rotation = Quaternion.Euler(lookAtPositions[currentIndex]);
        }
    }
}