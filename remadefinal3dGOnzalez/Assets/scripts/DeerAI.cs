using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DeerAI : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public Transform player;
    public float sightRange = 35f;
    public float projectileDetectRadius = 10f;
    public LayerMask obstacleMask;
    public Transform[] hidingSpots;

    public float baseSpeed = 6f;
    public float runSpeed = 25f;
    public float runDuration = 5f;

    private float wanderRadius = 100f;
    private float wanderTimer = 30f;
    private float timer;
    private float runTimer;
    private bool isRunning;
    private bool isHiding;
    private float hideWaitTime = 3f;
    private float hideWaitTimer;

    // Death / Scoring
    public GameObject deerBody;
    public static int killCount = 0;
    public GameObject scoreTextObj;
    private Text scoreText;
    private bool isDead;

    public BloodThirstManager bloodThirstManager;
    public Slider killSlider;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderTimer;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        hidingSpots = new Transform[trees.Length];
        for (int i = 0; i < trees.Length; i++) hidingSpots[i] = trees[i].transform;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name} is not on NavMesh.");
            enabled = false;
            return;
        }

        if (scoreTextObj != null) scoreText = scoreTextObj.GetComponent<Text>();
        if (killSlider != null) killSlider.value = killCount;

        Debug.Log($"{name} initialized.");
    }

    void Update()
    {
        if (isDead) return;

        timer += Time.deltaTime;

        if (!isRunning && timer >= wanderTimer)
        {
            Vector3 wanderPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(wanderPos);
            timer = 0;
        }

        if (isRunning)
        {
            runTimer += Time.deltaTime;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isHiding)
            {
                isHiding = true;
                hideWaitTimer = 0;
            }

            if (isHiding)
            {
                hideWaitTimer += Time.deltaTime;
                if (hideWaitTimer >= hideWaitTime)
                {
                    StopRunning();
                }
            }
        }

        CheckForPlayer();
    }

    void CheckForPlayer()
    {
        if (player == null || isDead) return;

        Vector3 direction = player.position - transform.position;
        if (Vector3.Distance(transform.position, player.position) < sightRange &&
            !Physics.Raycast(transform.position, direction.normalized, sightRange, obstacleMask))
        {
            RunAway();
        }
    }

    public void RunAway()
    {
        if (isRunning || isDead) return;

        agent.speed = runSpeed;
        for (int i = 0; i < 5; i++)
        {
            Transform randomSpot = hidingSpots[Random.Range(0, hidingSpots.Length)];
            if (NavMesh.SamplePosition(randomSpot.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                isRunning = true;
                return;
            }
        }

        Debug.LogWarning($"{name} failed to find a hiding spot.");
    }

    public void DetectProjectile(Vector3 position)
    {
        if (Vector3.Distance(position, transform.position) < projectileDetectRadius)
        {
            RunAway();
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randDirection = Random.insideUnitSphere * distance + origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, distance, NavMesh.AllAreas);
        return navHit.position;
    }

    public void StopRunning()
    {
        agent.speed = baseSpeed;
        isRunning = false;
        isHiding = false;
        runTimer = 0f;
        timer = wanderTimer;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Disable the BoxCollider immediately
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.enabled = false; // Disable the BoxCollider immediately
        }

        // Disable the Rigidbody immediately
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Set the Rigidbody to kinematic to stop physics interaction
        }

        animator?.SetBool("isDead", true);
        killCount++;

        if (scoreText != null)
            scoreText.text = "Kills: " + killCount;

        if (killSlider != null)
            killSlider.value = killCount;

        if (deerBody != null) deerBody.SetActive(false);
        else gameObject.SetActive(false);

        if (bloodThirstManager != null)
            bloodThirstManager.OnKill();
    }



    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Projectile") || other.CompareTag("Melee"))
        {
            DetectProjectile(other.transform.position);
            animator?.SetBool("isDead", true);
            Destroy(gameObject, 2f);

            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            scoreManager?.IncreaseKillCount(1);
        }
    }
}
