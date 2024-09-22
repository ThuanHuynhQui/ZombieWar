using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] Vector3VariableSO currentPlayerPositionSO;
    // Start is called before the first frame update
    void Start()
    {
        // navMeshAgent.updatePosition = false;
        // navMeshAgent.updateRotation = false;
        navMeshAgent.speed = characterMovement.Speed;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDestination();
        UpdateDirection();
        UpdateNavMeshAgent();
    }

    void UpdateDestination()
    {
        if (!navMeshAgent) return;
        navMeshAgent.SetDestination(currentPlayerPositionSO.Value);
    }

    void UpdateDirection()
    {
        if (!characterMovement) return;
        if (characterMovement.IsStop) return;
        characterMovement.MoveDirection = navMeshAgent.desiredVelocity;
    }

    void UpdateNavMeshAgent()
    {
        navMeshAgent.isStopped = characterMovement.IsStop;
    }
}
