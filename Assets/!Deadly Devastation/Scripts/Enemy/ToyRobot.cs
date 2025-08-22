using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class ToyRobot : MonoBehaviour, IHittable
{
    [Header("Audio")]
    [SerializeField] private AudioSource aud1;
    [SerializeField] private AudioSource aud2;
    [SerializeField] private AudioSource aud3;

    [Header("AI")]
    [SerializeField] private float minStopTime = 2f;
    [SerializeField] private float maxStopTime = 5f;
    [SerializeField] private float minRunTime = 3f;
    [SerializeField] private float maxRunTime = 5f;

    [SerializeField] private float agentSpeed = 3.5f;
    [SerializeField] private float agentAcceleration = 8f;
    [SerializeField] private float agentAngularSpeed = 120f;
    [SerializeField] private float stoppingDistance = 0.5f;

    private Transform closestPlayer;
    private NavMeshAgent aiAgent;
    private Animator anim;
    private Health health;
    private Coroutine behaviourRoutine;
    private bool isDead;

    private void Awake()
    {
        aiAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        aiAgent.speed = agentSpeed;
        aiAgent.acceleration = agentAcceleration;
        aiAgent.angularSpeed = agentAngularSpeed;
        aiAgent.stoppingDistance = stoppingDistance;
        aiAgent.updateRotation = true;
        aiAgent.updatePosition = true;

        anim.applyRootMotion = false;

        health.OnDied += OnDeath;
        Debug.Log("[ToyRobot] Awake: Components initialized");
    }

    private void Start()
    {
        aud3.enabled = false;
        anim.SetBool("IsRun", false);
        behaviourRoutine = StartCoroutine(BehaviourLoop());
        Debug.Log("[ToyRobot] Start: Behaviour loop started");
    }

    private void Update()
    {
        if (isDead) return;

        if (closestPlayer == null || !closestPlayer.gameObject.activeInHierarchy)
            FindClosestPlayer();

        if (closestPlayer != null && !aiAgent.isStopped)
        {
            aiAgent.SetDestination(closestPlayer.position);
            Debug.DrawLine(transform.position, closestPlayer.position, Color.red);
        }

        anim.SetBool("IsRun", !aiAgent.isStopped && aiAgent.velocity.magnitude > 0.1f);
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistanceSqr = Mathf.Infinity;
        GameObject closestPlayerObj = null;

        foreach (GameObject player in players)
        {
            float dSqrToTarget = (player.transform.position - transform.position).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestPlayerObj = player;
            }
        }

        if (closestPlayerObj != null)
        {
            closestPlayer = closestPlayerObj.transform;
            Debug.Log("[ToyRobot] Closest player found: " + closestPlayer.name);
        }
        else
        {
            Debug.Log("[ToyRobot] No players found!");
        }
    }

    private IEnumerator BehaviourLoop()
    {
        while (!isDead)
        {
            yield return StopState();
            yield return GoState();
        }
    }

    private IEnumerator StopState()
    {
        aiAgent.isStopped = true;
        aud3.enabled = false;
        aud1.Play();

        Debug.Log("[ToyRobot] StopState: Agent stopped");

        yield return new WaitForSeconds(Random.Range(minStopTime, maxStopTime));

        aud1.Stop();
        Debug.Log("[ToyRobot] StopState: Finished waiting");
    }

    private IEnumerator GoState()
    {
        aiAgent.isStopped = false;
        aud3.enabled = true;
        aud2.Play();

        Debug.Log("[ToyRobot] GoState: Agent moving");

        yield return new WaitForSeconds(Random.Range(minRunTime, maxRunTime));

        aud2.Stop();
        Debug.Log("[ToyRobot] GoState: Finished running");
    }

    public void OnHit(Vector3 force, int damage, GameObject hitter)
    {
        if (isDead) return;

        health.TakeDamage(damage);
        Debug.Log("[ToyRobot] OnHit: Took " + damage + " damage from " + hitter.name);
    }

    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        if (behaviourRoutine != null)
            StopCoroutine(behaviourRoutine);

        aiAgent.isStopped = true;
        aiAgent.enabled = false;

        aud1.Stop();
        aud2.Stop();
        aud3.enabled = false;

        AudioManager.Instance.PlayAndForget("Deathblow");

        anim.SetBool("IsRun", false);
        anim.SetTrigger("Die");

        Debug.Log("[ToyRobot] OnDeath: Robot died");

        StartCoroutine(DisableAnimatorAndSink());
    }

    private IEnumerator DisableAnimatorAndSink()
    {
        yield return new WaitForSeconds(5f);
        anim.enabled = false;
        Debug.Log("[ToyRobot] DisableAnimatorAndSink: Animator disabled");

        yield return SinkObject();
    }

    private IEnumerator SinkObject()
    {
        float sinkDuration = 4f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * 3f;

        while (elapsed < sinkDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        Debug.Log("[ToyRobot] SinkObject: Destroying robot");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (aiAgent != null && closestPlayer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, aiAgent.destination);
        }
    }
}
