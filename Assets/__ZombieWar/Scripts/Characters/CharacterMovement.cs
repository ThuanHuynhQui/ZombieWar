using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMovement : MonoBehaviour
{
    [Header("Tweak")]
    [SerializeField] protected float speed = 5;
    [Header("References")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [HideInInspector] public Vector3 MoveDirection;

    public float Speed => speed;
    public bool IsStop = false;

    private void Update()
    {
        UpdateAnimatorVelocity();
    }

    protected virtual void FixedUpdate()
    {
        if (IsStop) return;
        RotateToMoveDirection();
        MoveForward();
    }

    //Rotate character to the desired direction.
    void RotateToMoveDirection()
    {
        if (MoveDirection == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(MoveDirection.normalized, Vector3.up);
        rb.MoveRotation(targetRotation);
    }

    //Apply velocity forward
    void MoveForward()
    {
        rb.velocity = MoveDirection.normalized.magnitude * speed * transform.forward;
    }

    void UpdateAnimatorVelocity()
    {
        animator.SetFloat(GlobalConst.AnimatorVelStr, rb.velocity.magnitude);
    }
}
