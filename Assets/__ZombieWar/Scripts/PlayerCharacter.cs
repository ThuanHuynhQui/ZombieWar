using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerCharacter : Character
{
    [SerializeField] Transform aimingTransform;
    [SerializeField] Rig rig;
    [SerializeField] LayerMask overlapSphereDetectLayer;
    [SerializeField] LayerMask raycastDetectLayer;
    private float DetectionRange => 5;
    private Character currentTarget;
    private float aimingHeight => aimingTransform ? aimingTransform.position.y : 0;
    public override Character CurrentTarget => currentTarget;

    // Update is called once per frame
    private void Update()
    {
        UpdateRig();
    }

    void FixedUpdate()
    {
        DetectTargetInRange();
        AimingAtTarget();
    }

    void AimingAtTarget()
    {
        if (CurrentTarget == null) return;
        Vector3 aimDirection = CurrentTarget.transform.position;
        // aimDirection = aimDirection.normalized;
        aimingTransform.position = new Vector3(aimDirection.x, aimingTransform.position.y, aimDirection.z);
    }

    void AttackTarget()
    {
        if (CurrentTarget == null) return;

    }

    void AttachWeapon()
    {

    }

    void UpdateRig()
    {
        if (!rig) return;
        rig.weight = CurrentTarget ? 1 : 0;
    }

    void DetectTargetInRange()
    {
        if (DetectionRange <= 0) return;
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, DetectionRange, overlapSphereDetectLayer);
        if (enemiesInRange.Length > 0)
        {
            SortEnemiesByDistance();
            Collider closestEnemy = GetClosestAttackableEnemy();
            if (closestEnemy != null)
            {
                currentTarget = GetClosestAttackableEnemy().GetComponent<Character>();
            }
            else
            {
                currentTarget = null;
            }
        }
        else
        {
            currentTarget = null;
        }

        void SortEnemiesByDistance()
        {
            //Sort the enemies array from closest to furthest
            for (int i = 0; i < enemiesInRange.Length - 1; i++)
            {
                bool hasSwapped = false;
                for (int j = 0; j < enemiesInRange.Length - i - 1; j++)
                {
                    float distanceFromEnemy1 = Vector3.Distance(enemiesInRange[j].transform.position, transform.position);
                    float distanceFromEnemy2 = Vector3.Distance(enemiesInRange[j + 1].transform.position, transform.position);
                    if (distanceFromEnemy1 > distanceFromEnemy2)
                    {
                        //Swap two enemies
                        Collider temp = enemiesInRange[j];
                        enemiesInRange[j] = enemiesInRange[j + 1];
                        enemiesInRange[j + 1] = temp;
                        hasSwapped = true;
                    }
                }
                if (!hasSwapped)
                {
                    break;
                }
            }
        }

        Collider GetClosestAttackableEnemy()
        {
            foreach (var enemy in enemiesInRange)
            {
                Vector3 rayStartPos = new(transform.position.x, aimingHeight, transform.position.z);
                Ray ray = new(rayStartPos, enemy.transform.position - rayStartPos);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, DetectionRange, raycastDetectLayer))
                {
                    if (hitInfo.collider == enemy) return enemy; //Nothing block the enemy
                    else continue;
                }
            }
            return null; //Could not find any attackable enemy.
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (DetectionRange == 0) return;
        Color gizmoColor = Color.yellow;
        gizmoColor.a = 0.5f;
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, DetectionRange);
    }
#endif
}
