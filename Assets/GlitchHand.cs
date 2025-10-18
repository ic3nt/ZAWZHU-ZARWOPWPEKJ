using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GlitchHandFinalAI : MonoBehaviour
{
    public enum HandState { Idle, Wander, Chase }
    private HandState state = HandState.Wander;

    [Header("Movement")]
    public float wanderRadius = 8f;
    public float wanderInterval = 4f;
    public float baseHoverHeight = 1.5f;
    public float hoverAmplitude = 0.3f;
    public float hoverSpeed = 2f;
    public float turnSpeed = 5f;

    [Header("Player Detection")]
    public float detectRange = 10f;
    public float chaseRange = 6f;
    public float loseRange = 12f;
    public float wanderSpeed = 2.5f;
    public float chaseSpeed = 5f;

    [Header("Glitch Behavior")]
    public bool enableGlitch = true;
    public float positionJitterAmount = 0.05f;
    public float rotationJitterDegrees = 2f;
    public float dashChance = 0.25f;
    public float dashDistance = 3f;
    public float dashCooldown = 3f;
    public float teleportChance = 0.1f;
    public float teleportDistance = 5f;

    private Transform player;
    private NavMeshAgent agent;
    private Vector3 wanderTarget;
    private float lastWanderTime;
    private float hoverOffset;
    private float nextDashTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.autoBraking = false;
        agent.speed = wanderSpeed;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        hoverOffset = Random.Range(0f, Mathf.PI * 2f);
        ChooseNewWanderTarget();
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case HandState.Wander:
                Wander(dist);
                break;
            case HandState.Chase:
                Chase(dist);
                break;
        }

        HoverEffect();
        UpdateRotation();
        if (enableGlitch) ApplyGlitchEffect();
    }

    // --------------------------
    //         LOGIC
    // --------------------------

    void Wander(float dist)
    {
        agent.speed = wanderSpeed;

        if (dist <= detectRange)
        {
            state = HandState.Chase;
            return;
        }

        if (Time.time - lastWanderTime > wanderInterval || Vector3.Distance(transform.position, wanderTarget) < 1f)
            ChooseNewWanderTarget();

        agent.SetDestination(wanderTarget);
    }

    void Chase(float dist)
    {
        agent.speed = chaseSpeed;

        if (dist > loseRange)
        {
            state = HandState.Wander;
            ChooseNewWanderTarget();
            return;
        }

        // Всегда идёт на игрока
        agent.SetDestination(player.position);

        // Случайный рывок / телепорт к игроку
        TryDashOrTeleport();
    }

    // --------------------------
    //     EFFECTS & ROTATION
    // --------------------------

    void HoverEffect()
    {
        Vector3 pos = transform.position;
        pos.y = baseHoverHeight + Mathf.Sin(Time.time * hoverSpeed + hoverOffset) * hoverAmplitude;
        transform.position = pos;
    }

    void UpdateRotation()
    {
        Vector3 direction = Vector3.zero;

        // Направление к игроку в режиме погони, иначе по направлению движения
        if (state == HandState.Chase && player != null)
            direction = (player.position - transform.position).normalized;
        else if (agent.velocity.sqrMagnitude > 0.1f)
            direction = agent.velocity.normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion smoothRot = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);

            // Фиксируем -90 градусов по Z
            transform.rotation = Quaternion.Euler(smoothRot.eulerAngles.x, smoothRot.eulerAngles.y, -90f);
        }
    }

    void ApplyGlitchEffect()
    {
        float t = Time.time * 20f;
        Vector3 smallJitter = new Vector3(
            Mathf.PerlinNoise(t, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, t) - 0.5f,
            Mathf.PerlinNoise(t * 0.5f, t * 0.3f) - 0.5f
        ) * positionJitterAmount * 2f;

        transform.localPosition += smallJitter;

        Vector3 rotJitter = new Vector3(
            (Mathf.PerlinNoise(t * 0.7f, 1f) - 0.5f),
            (Mathf.PerlinNoise(1f, t * 0.5f) - 0.5f),
            (Mathf.PerlinNoise(t * 0.3f, 0.8f) - 0.5f)
        ) * rotationJitterDegrees;

        transform.localRotation *= Quaternion.Euler(rotJitter);
    }

    void TryDashOrTeleport()
    {
        if (Time.time < nextDashTime) return;
        nextDashTime = Time.time + dashCooldown;

        Vector3 toPlayer = (player.position - transform.position).normalized;

        // 50% шанс рывка или телепорта
        if (Random.value < dashChance)
        {
            transform.position += toPlayer * dashDistance;
        }
        else if (Random.value < teleportChance)
        {
            transform.position += toPlayer * teleportDistance;
        }
    }

    void ChooseNewWanderTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            wanderTarget = hit.position;

        lastWanderTime = Time.time;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
