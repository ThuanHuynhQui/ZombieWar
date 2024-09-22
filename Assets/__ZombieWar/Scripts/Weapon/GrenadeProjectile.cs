using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : Projectile
{


    [Range(1f, 99f)] public float GravityMultiplier = 1f;
    public float MaxY = 4f;

    public Vector3 Gravity => Physics.gravity * GravityMultiplier;

    public override void Launch(Vector3 direction, float speed)
    {
        transform.parent = null;
        cld.enabled = true;
        rb.isKinematic = false;
        rb.velocity = GetThrowDirection(direction);
        launching = true;
    }

    protected override void FixedUpdate()
    {
        if (!launching) return;
        // Calculate the force required to counteract global gravity
        Vector3 counterForce = (Physics.gravity.y * GravityMultiplier) * rb.mass * Vector3.up - Physics.gravity * rb.mass;
        // Apply the counteracting force to the object
        rb.AddForce(counterForce, ForceMode.Force);
    }

    Vector3 GetThrowDirection(Vector3 target)
    {
        Vector3 v0 = new();
        var pos0 = transform.position;
        // Equation:
        // y = pos0.y + v0.y * t + 0.5 * Gravity.y * t^2
        // x = pos0.x + v0.x * t
        // z = pos0.z + v0.z * t

        // tHighest is the time to reach MaxY
        // 0 = v0.y + Gravity.y * tHighest => tHighest = -v0.y / Gravity.y
        // Apply to MaxY = pos0.y + v0.y * tHighest + 0.5 * Gravity.y * tHighest^2 => MaxY = pos0.y - v0.y^2 / Gravity.y + 0.5 * Gravity.y * v0.y^2 / Gravity.y^2
        v0.y = Mathf.Sqrt((MaxY - pos0.y) / (-0.5f / Gravity.y));

        // tMax is the time to reach target
        // target.y = pos0.y + v0.y * tMax + 0.5 * Gravity.y * tMax^2
        MathHelper.SolveQuadratic(0.5f * Gravity.y, v0.y, pos0.y - target.y, out var x1, out var x2);
        var tMax = Mathf.Max(x1, x2); // Get greater value

        // Calculate v0.x, v0.z
        v0.x = (target.x - pos0.x) / tMax;
        v0.z = (target.z - pos0.z) / tMax;
        return v0;
    }
}

public static class MathHelper
{
    public static void SolveQuadratic(float a, float b, float c, out float x1, out float x2)
    {
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            throw new System.Exception("No solution");
        }
        var sqrtDiscriminant = Mathf.Sqrt(discriminant);
        x1 = (-b + sqrtDiscriminant) / (2 * a);
        x2 = (-b - sqrtDiscriminant) / (2 * a);
    }
}