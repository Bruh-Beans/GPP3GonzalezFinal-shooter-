using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;

    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    // Head bobbing parameters
    public float bobbingSpeed = 0.18f; // Speed of the bobbing
    public float bobbingAmount = 0.1f; // Amount of bobbing
    private float timeSinceLastStep = 0f; // Time tracker for bobbing

    private float mouseX;
    private float mouseY;
    private float offsetDistanceY;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = transform.position.y;

        // Lock and hide cursor with option isn't checked
        if (!clickToMoveCamera)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    void Update()
    {
        // Follow player - camera offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Set camera zoom when mouse wheel is scrolled
        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;

        // Checker for right click to move camera
        if (clickToMoveCamera)
            if (Input.GetAxisRaw("Fire2") == 0)
                return;

        // Calculate new position
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;

        // Apply camera limits
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Set the camera rotation
        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

        // Apply head bobbing when player is moving
        ApplyHeadBobbing();
    }

    // Function to apply head bobbing based on player movement
    void ApplyHeadBobbing()
    {
        // Check if the player is moving (you can adjust this based on your movement logic)
        if (player.GetComponent<Rigidbody>().velocity.magnitude > 0.1f) // Threshold to trigger bobbing
        {
            timeSinceLastStep += Time.deltaTime * bobbingSpeed;
            float newY = Mathf.Sin(timeSinceLastStep) * bobbingAmount;

            // Apply vertical movement for head bobbing
            transform.localPosition = new Vector3(transform.localPosition.x, offsetDistanceY + newY, transform.localPosition.z);
        }
        else
        {
            // Reset the head bobbing when the player stops moving
            timeSinceLastStep = 0f;
            transform.localPosition = new Vector3(transform.localPosition.x, offsetDistanceY, transform.localPosition.z);
        }
    }
}
