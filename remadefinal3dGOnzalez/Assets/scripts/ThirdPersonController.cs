using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThirdPersonController : MonoBehaviour
{

    private float fixedYPosition;


    [Header("UI Elements")]
    public TextMeshProUGUI meleeMessageText;
    public TextMeshProUGUI meleeTimerText;

    [Header("Melee Timer")]
    public float meleeDuration = 20f;
    private float meleeTimeLeft = 0f;
    private bool isMeleeTimerRunning = false;
    private float meleeCooldown = 0.68f;
    private float nextMeleeTime = 0f;


    public GameObject objectToMove;
    [Header("Bow / Projectile")]
    public GameObject bowWeapon;
    public Slider chargeBar;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 20f;
    private float shootCooldown = 0.5f;
    private float nextShootTime = 0f;
    private float currentCharge = 0f;
    public float maxCharge = 1f, minProjectileSpeed = 0f, maxProjectileSpeed = 100f;
    private bool isCharging = false, isDrawn = false;

    [Header("Movement")]
    public float velocity = 5f, sprintAddition = 3.5f;
    private float inputHorizontal, inputVertical;
    private bool inputSprint, isWalking, isSprinting;

    [Header("Camera & FOV")]
    public Camera thirdPersonCam;
    public Camera firstPersonCam;
    public float normalFOV = 80f, zoomedFOV = 40f, fovTransitionSpeed = 5f;

    [Header("BloodThirst")]
    public float bloodThirstSprintAddition = 5f;
    private float defaultSprintAddition;

    [Header("Melee")]
    public GameObject meleeWeapon;
    public int killsToSwitch = 5;
    public float meleeSpeedBoost = 50f;
    public bool isMeleeMode = false;
    private bool isWeaponEquipped = false;
    private float defaultVelocity;

    private Animator animator;
    private CharacterController cc;

    void Start()
    {
        if (thirdPersonCam) thirdPersonCam.enabled = true;
        if (firstPersonCam) firstPersonCam.enabled = false;

        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        defaultSprintAddition = sprintAddition;
        defaultVelocity = velocity;

        bowWeapon?.SetActive(true);
        meleeWeapon?.SetActive(false);
        fixedYPosition = transform.position.y;

    }

    void Update()
    {
        if (!isMeleeMode && ScoreManager.Instance != null && ScoreManager.Instance.totalKillCount >= killsToSwitch)
        {
            SwitchToMeleeMode();
        }


        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputSprint = Input.GetAxis("Fire3") == 1f;
        isWalking = Mathf.Abs(inputHorizontal) > 0.1f || Mathf.Abs(inputVertical) > 0.1f;
        isSprinting = inputSprint && isWalking;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isSprinting", isSprinting);

        if (!isMeleeMode)
        {
            bowWeapon?.SetActive(true);
            meleeWeapon?.SetActive(false);

            if (projectileSpawnPoint && Camera.main)
                projectileSpawnPoint.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            if (chargeBar) chargeBar.value = currentCharge;

            HandleShooting();
            HandleZoomFOV();
        }
        else
        {
            bowWeapon?.SetActive(false);
            chargeBar?.gameObject.SetActive(false);
            projectileSpawnPoint?.gameObject.SetActive(false);
            meleeWeapon?.SetActive(true);

            animator.SetBool("isSwordRunning", isWalking);
            animator.SetBool("isSwordIdle", !isWalking);

            if (isCharging)
            {
                isCharging = false;
                isDrawn = false;
                animator.SetBool("isCharging", false);
                animator.SetBool("isDrawn", false);
            }

            if (Input.GetMouseButtonDown(0) && Time.time >= nextMeleeTime)
            {
                AttackWithMeleeWeapon();
                nextMeleeTime = Time.time + meleeCooldown;
            }

        }

        if (isMeleeTimerRunning)
        {
            meleeTimeLeft -= Time.deltaTime;
            meleeTimerText.text = "BloodThirst: " + Mathf.CeilToInt(meleeTimeLeft).ToString();

            if (meleeTimeLeft <= 0f)
            {
                isMeleeTimerRunning = false;
                meleeTimerText.text = "";
                SwitchToRangedMode();
            }
        }
    }

    void FixedUpdate()
    {
        float speed = velocity + (isSprinting ? sprintAddition : 0f);

        // Use the correct camera depending on melee mode
        Camera activeCam = isMeleeMode && firstPersonCam != null ? firstPersonCam : thirdPersonCam;

        Vector3 fwd = activeCam.transform.forward;
        fwd.y = 0;

        if (fwd.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fwd), 0.15f);

        Vector3 move = fwd * (inputVertical * speed * Time.deltaTime) +
                       activeCam.transform.right * (inputHorizontal * speed * Time.deltaTime);
        move.y = 0;

        cc.Move(move);

        // Force Y position to stay the same
        Vector3 correctedPos = transform.position;
        correctedPos.y = fixedYPosition;
        transform.position = correctedPos;
    }

    void HandleZoomFOV()
    {
        float targetFOV = Input.GetMouseButton(1) ? zoomedFOV : normalFOV;
        thirdPersonCam.fieldOfView = Mathf.Lerp(thirdPersonCam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextShootTime)
        {
            isCharging = true;
            currentCharge = 0f;
            isDrawn = true;
            animator.SetBool("isCharging", true);
            animator.SetBool("isDrawn", true);
            nextShootTime = Time.time + shootCooldown;

        }

        if (Input.GetKey(KeyCode.Mouse0) && isCharging)
        {
            currentCharge = Mathf.Clamp(currentCharge + Time.deltaTime, 0f, maxCharge);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && isCharging)
        {
            isCharging = false;
            isDrawn = false;
            animator.SetBool("isCharging", false);
            animator.SetBool("isDrawn", false);
            animator.SetTrigger("Shoot");

            float speed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, currentCharge / maxCharge);
            Quaternion rot = projectileSpawnPoint.rotation * Quaternion.Euler(90, 0, 0);
            GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, rot);

            if (proj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = projectileSpawnPoint.forward * speed;
            }

            currentCharge = 0f;
        }
    }
    public bool IsMeleeModeActive()
    {
        return isMeleeMode;
    }


    void AttackWithMeleeWeapon()
    {
        if (!isWeaponEquipped) return;
        animator.SetTrigger("isSwordAttack");
    }

    IEnumerator ShowMeleeMessage(string message, float duration)
    {
        meleeMessageText.text = message;
        meleeMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        meleeMessageText.gameObject.SetActive(false);
    }

    public void SwitchToMeleeMode()
    {
        isMeleeMode = true;
        isWeaponEquipped = true;
        animator.SetBool("isMelee", true);
        velocity += meleeSpeedBoost;

        if (thirdPersonCam) thirdPersonCam.enabled = false;
        if (firstPersonCam) firstPersonCam.enabled = true;

        // Move the object
        if (objectToMove != null)
        {
            Vector3 currentPosition = objectToMove.transform.position;
            objectToMove.transform.position = new Vector3(currentPosition.x, currentPosition.y - 10f, currentPosition.z);
        }

        // Show message
        StartCoroutine(ShowMeleeMessage("BloodThirst Activated!", 2f));

        // Start timer
        meleeTimeLeft = meleeDuration;
        isMeleeTimerRunning = true;
    }

    public void SwitchToRangedMode()
    {
        isMeleeMode = false;
        isWeaponEquipped = false;
        animator.SetBool("isMelee", false);
        velocity = defaultVelocity;

        // Camera switch back
        if (firstPersonCam) firstPersonCam.enabled = false;
        if (thirdPersonCam) thirdPersonCam.enabled = true;

        // Re-enable bow and projectile stuff
        bowWeapon?.SetActive(true);
        chargeBar?.gameObject.SetActive(true);
        projectileSpawnPoint?.gameObject.SetActive(true);

        // Reset animator melee states
        animator.SetBool("isSwordRunning", false);
        animator.SetBool("isSwordIdle", false);
        ScoreManager.Instance.totalKillCount = 0; // Or add a boolean flag to prevent re-entry

    }

    public void IncreaseSprintSpeed() => sprintAddition = bloodThirstSprintAddition;
    public void ResetSprintSpeed() => sprintAddition = defaultSprintAddition;

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.8f);
    }
}





