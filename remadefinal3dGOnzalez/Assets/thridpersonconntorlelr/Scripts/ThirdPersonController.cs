using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    private Vector3 defaultAttachPosition;

    public Slider chargeBar;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 20f;

    // Bow charging system variables...
    float currentCharge = 0f;
    public float maxCharge = 1f;
    public float minProjectileSpeed = 0;
    public float maxProjectileSpeed = 100f;
    bool isCharging = false;

    [Tooltip("Speed at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("Value added to the speed while sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Space]
    [Tooltip("Force that pulls the player down.")]
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;

    // Player states
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    // --- New camera variables for aiming ---
    public Camera thirdPersonCam;
    public Camera aimCam;
    private bool isAiming = false;
    // --- End camera variables ---

    void Start()
    {
        defaultAttachPosition = projectileSpawnPoint.localPosition;

        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Animator component not found. Animations won't work.");
    }

    void Update()
    {
        // Rotate attach point to match camera's forward direction
        if (projectileSpawnPoint != null && Camera.main != null)
        {
            projectileSpawnPoint.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }


        if (chargeBar != null)
            chargeBar.value = currentCharge;

        HandleShooting();

        // Input handling...
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1);
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Other animation and jump code...
        if (inputJump && cc.isGrounded)
            isJumping = true;

        // --- Camera switching via right-click ---
       if (Input.GetMouseButtonDown(1))  // Right click pressed
{
    isAiming = true;
    thirdPersonCam.enabled = false;
    aimCam.enabled = true;

    // Move attach point to align with crosshair while aiming
    projectileSpawnPoint.localPosition = new Vector3(0.583f, 0.632f, -3.34f);
}
else if (Input.GetMouseButtonUp(1))  // Right click released
{
    isAiming = false;
    thirdPersonCam.enabled = true;
    aimCam.enabled = false;

    // Reset attach point position
    projectileSpawnPoint.localPosition = defaultAttachPosition;
}


        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        // Process movement input...
        float velocityAdittion = isSprinting ? sprintAdittion : 0;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.50f);

        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        // Jump logic...
        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }
        directionY -= gravity * Time.deltaTime;

        // --- Character rotation based on the active camera ---
        Camera activeCamera = isAiming ? aimCam : thirdPersonCam;
        Vector3 cameraForward = activeCamera.transform.forward;
        cameraForward.y = 0;
        if (cameraForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
        // --- End rotation ---

        // Continue with movement...
        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = activeCamera.transform.forward * directionZ + activeCamera.transform.right * directionX;
        cc.Move(verticalDirection + horizontalDirection);
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;
        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isCharging = true;
            currentCharge = 0f;
        }
        if (Input.GetKey(KeyCode.Mouse0) && isCharging)
        {
            currentCharge += Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && isCharging)
        {
            isCharging = false;
            float chargePercent = currentCharge / maxCharge;
            float finalSpeed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargePercent);

            Quaternion spawnRotation = projectileSpawnPoint.rotation * Quaternion.Euler(90, 0, 0);
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, spawnRotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = projectileSpawnPoint.forward * finalSpeed;

            // Optional: Rotate the player toward target based on mouse raycast...
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.3f);
                }
            }
            currentCharge = 0f;
        }
    }

}
