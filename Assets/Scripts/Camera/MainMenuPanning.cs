using UnityEngine;

public class MainMenuPanning : MonoBehaviour
{
    public float speed = 2f;

    private int currentIndex = 0;

    // Camera starting positions
    Vector3[] startingPositions =
    {
        new Vector3(0, 5, -10),
        new Vector3(-1.59f, 3.28f, -6.35f),
        new Vector3(20, 5, -10),
        new Vector3(30, 5, -10)
    };

    // Camera target positions to pan to
    Vector3[] targetPositions =
    {
        new Vector3(0, 5, 0),
        new Vector3(-1.59f, 3.28f, 6.76f),
        new Vector3(20, 5, 0),
        new Vector3(30, 5, 0)
    };

    // Camera Rotation look at positions
    Vector3[] lookAtPositions =
    {
        new Vector3(0, 0, 0),
        new Vector3(16.71f, -90.0f, 0.0f),
        new Vector3(20, 0, 0),
        new Vector3(30, 0, 0)
    };

    void Start()
    {
        currentIndex = 0;
        GetComponent<Camera>().transform.position = startingPositions[currentIndex];
        GetComponent<Camera>().transform.LookAt(lookAtPositions[currentIndex]);
    }

    void Update()
    {

        // Check if the camera has reached the target position
        if (GetComponent<Camera>().transform.position != targetPositions[currentIndex])
        {
            // Move the camera towards the target position
            GetComponent<Camera>().transform.position = Vector3.MoveTowards(GetComponent<Camera>().transform.position, targetPositions[currentIndex], speed * Time.deltaTime);
        }
        else
        {
            currentIndex = (currentIndex + 1) % targetPositions.Length; // Reset to 0 after reaching the end

            // Set the camera to the new starting position and look at the new target
            GetComponent<Camera>().transform.position = startingPositions[currentIndex];
            GetComponent<Camera>().transform.rotation = Quaternion.Euler(lookAtPositions[currentIndex]);
        }
    }
}