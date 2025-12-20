using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public enum AIState
{
    Patrol,
    Suspicious,
    Alert,
    Searching
}

public class Enemyai : MonoBehaviour
{
    private AIState previousState;
    private AIState currentState = AIState.Patrol;
    [Header("Light Settings")]
    public Light aiLight;
    public Color patrolColor = Color.white;
    public Color suspiciousColor = Color.yellow;
    public Color alertColor = Color.red;

    public float alertConfirmTime = 1.0f;
    private float alertTimer = 0f;

    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float lostPlayerCooldown = 5f;
    public List<Transform> patrolPoints;
    public LayerMask playerLayer;
    public float searchRotateSpeed = 120f;
    
    [Header("Movement Settings")]
    public float normalSpeed = 3.5f;
    public float susspeed = 2.0f;
    public float chasingspeed = 4.0f;




    private int currentPatrolIndex = 0;
    private Transform targetPlayer = null;
    private Vector3 lastSeenPosition;
    private float lostPlayerTimer = 0f;
    private bool searching = false;
    private int patrolDirection = 1; // 1 = forward, -1 = backward
    [Header("Vision Settings")]
    public float viewAngle = 90f; // total cone angle (e.g. 90 = 45 left, 45 right)
    public float lastSeenReachDistance = 0.5f;

    [Header("SFX Settings")]
    public AudioSource RobotAudioSource;

    public AudioClip alertAudioTrack;
    public AudioClip suspiciousAudioTrack;
    public AudioClip findplayerAudioTrack;


    // public Audio chaseAudioTrack;
    // public Audio patrolAudioTrack;


    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        previousState = currentState;
    }

    void HandleStateEnter(AIState newState)
    {
        if (agent == null) return;

        switch (newState)
        {
            case AIState.Patrol:
                agent.speed = normalSpeed;
                break;

            case AIState.Suspicious:
                agent.speed = susspeed;
                PlaySFX(suspiciousAudioTrack);
                break;

            case AIState.Alert:
                agent.speed = chasingspeed;
                PlaySFX(alertAudioTrack);
                break;

            case AIState.Searching:
                agent.speed = susspeed;
                PlaySFX(findplayerAudioTrack);
                break;
        }
    }


    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        RobotAudioSource.Stop();
        RobotAudioSource.clip = clip;
        RobotAudioSource.Play();
    }



    void Update()
    {
        Transform nearestPlayer = FindNearestPlayerInSight();

        switch (currentState)
        {
            case AIState.Patrol:
                SetLight(patrolColor);
                Patrol();

                if (nearestPlayer != null)
                {
                    targetPlayer = nearestPlayer;
                    alertTimer = 0f;
                    agent.isStopped = true;
                    currentState = AIState.Suspicious;
                }
                break;

            case AIState.Suspicious:
                SetLight(suspiciousColor);

                if (nearestPlayer != null)
                {
                    FaceTargetFast(nearestPlayer.position);
                    lastSeenPosition = nearestPlayer.position;
                    alertTimer += Time.deltaTime;

                    if (alertTimer >= alertConfirmTime)
                    {
                        agent.isStopped = false;
                        currentState = AIState.Alert;
                    }
                }
                else
                {
                    agent.isStopped = false;
                    MoveTo(lastSeenPosition);

                    float dist = Vector3.Distance(
                        new Vector3(transform.position.x, 0, transform.position.z),
                        new Vector3(lastSeenPosition.x, 0, lastSeenPosition.z)
                    );

                    if (dist <= lastSeenReachDistance)
                    {
                        lostPlayerTimer = 0f;
                        agent.isStopped = true;
                        currentState = AIState.Searching;
                    }
                }
                break;



            case AIState.Alert:
                SetLight(alertColor);

                if (nearestPlayer != null)
                {
                    targetPlayer = nearestPlayer;
                    lastSeenPosition = targetPlayer.position;

                    agent.isStopped = false;
                    FaceTargetFast(targetPlayer.position);
                    ChasePlayer();
                }
                else
                {
                    agent.isStopped = false;
                    MoveTo(lastSeenPosition);

                    float dist = Vector3.Distance(
                        new Vector3(transform.position.x, 0, transform.position.z),
                        new Vector3(lastSeenPosition.x, 0, lastSeenPosition.z)
                    );

                    if (dist <= lastSeenReachDistance)
                    {
                        agent.isStopped = true;
                        lostPlayerTimer = 0f;
                        currentState = AIState.Searching;
                    }
                }
                break;



            case AIState.Searching:
                SetLight(suspiciousColor);

                Transform foundPlayer = FindNearestPlayerInSight();
                if (foundPlayer != null)
                {
                    targetPlayer = foundPlayer;
                    alertTimer = 0f;
                    agent.isStopped = true;
                    currentState = AIState.Suspicious;
                    break;
                }

                lostPlayerTimer += Time.deltaTime;
                transform.Rotate(Vector3.up, searchRotateSpeed * Time.deltaTime);
                agent.isStopped = true;

                if (lostPlayerTimer >= lostPlayerCooldown)
                {
                    agent.isStopped = false;
                    targetPlayer = null;
                    lostPlayerTimer = 0f;
                    currentState = AIState.Patrol;
                }
                break;

        }

        if (currentState != previousState)
        {
            HandleStateEnter(currentState);
            previousState = currentState;
        }

    }
    void FaceTargetFast(Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - transform.position);
        dir.y = 0f;

        if (dir == Vector3.zero) return;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * 12f // rotation speed
        );
    }


    Transform FindNearestPlayerInSight()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized;

            // 🔺 ANGLE CHECK (cone vision)
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle > viewAngle * 0.5f)
                continue; // outside vision cone

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            // 🔺 LINE OF SIGHT CHECK
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                                 directionToPlayer,
                                 out RaycastHit rayHit,
                                 detectionRange))
            {
                if (rayHit.collider == hit)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;
                        nearest = hit.transform;
                    }
                }
            }
        }

        return nearest;
    }


    void ChasePlayer()
    {
        if (targetPlayer == null) return;
        MoveTo(targetPlayer.position);
        Debug.Log("Chasing player at position: " + targetPlayer.position);
    }

    void MoveTo(Vector3 destination)
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        MoveTo(patrolTarget.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Check if we reached the end
            if (currentPatrolIndex == patrolPoints.Count - 1)
            {
                patrolDirection = -1; // go backward
            }
            // Check if we reached the start
            else if (currentPatrolIndex == 0)
            {
                patrolDirection = 1; // go forward
            }

            currentPatrolIndex += patrolDirection;
        }
    }
    void SetLight(Color color)
    {
        if (aiLight != null)
            aiLight.color = color;
    }


}
