using UnityEngine;

public class MainMenuPanning : MonoBehaviour
{
    public float speed = 2f;

    private int currentIndex = 0;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Camera starting positions
    public Vector3[] startingPositions =
    {
        new Vector3(4f, 5f, 0.65f),
        new Vector3(-1.59f, 3.28f, -6.35f),
        new Vector3(4.226f, 1.94f, -5.989f),
    };

    // Camera target positions to pan to
    public Vector3[] targetPositions =
    {
        new Vector3(4f, 5f, 0.65f),
        new Vector3(-1.59f, 3.28f, 6.76f),
        new Vector3(2.734f, 1.94f, -4.422f),
    };

    // Camera Rotation look at positions
    public Vector3[] lookAtPositions =
    {
        new Vector3(24.74f, -223.6f, 0f),
        new Vector3(16.71f, -90.0f, 0.0f),
        new Vector3(0, -223.6f, 0f),
    };

    // Camera Rotation target look at positions
    public Vector3[] targetLookAt =
    {
        new Vector3(24.74f, 313.9f, 0),
        new Vector3(16.71f, -90.0f, 0.0f),
        new Vector3(0, -223.6f, 0),
    };

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
            currentIndex = (currentIndex + 1) % targetPositions.Length; // Reset to 0 after reaching the end
            // Set the camera to the new starting position and look at the new target
            cam.transform.position = startingPositions[currentIndex];
            cam.transform.rotation = Quaternion.Euler(lookAtPositions[currentIndex]);
        }
    }
}