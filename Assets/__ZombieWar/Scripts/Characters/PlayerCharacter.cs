using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerCharacter : Character
{
    [SerializeField] List<Rig> rigs;
    [SerializeField] Transform aimingTransform;
    [SerializeField] FloatVariableSO playerHealthSO;
    [SerializeField] LayerMask overlapSphereDetectLayer;
    [SerializeField] LayerMask raycastDetectLayer;

    protected float DetectionRange => CurrentWeaponSO ? currentWeaponSO.WeaponRange : 0;
    protected float aimingHeight => aimingTransform ? aimingTransform.position.y : 0;
    protected Character currentTarget;
    public override Character CurrentTarget => currentTarget;
    protected override float CurrentHealth
    {
        get => playerHealthSO ? playerHealthSO.Value : 0;
        set
        {
            if (!playerHealthSO) return;
            playerHealthSO.Value = Mathf.Max(value, 0);
            base.CurrentHealth = value;
            if (playerHealthSO.Value <= 0) Die();
        }
    }
    protected Rig CurrentRig => CurrentWeaponSO ? rigs[(int)CurrentWeaponSO.WeaponType] : null;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PlayerWeaponEventCode.OnRequestWeaponChange, HandleWeaponChanged);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PlayerWeaponEventCode.OnRequestWeaponChange, HandleWeaponChanged);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDeath) return;
        AttackTarget();
    }

    void FixedUpdate()
    {
        if (isDeath) return;
        DetectTargetInRange();
        AimingAtTarget();
    }

    void AimingAtTarget()
    {
        Vector3 aimDirection;
        if (CurrentTarget != null)
        {
            aimDirection = CurrentTarget.transform.position;
        }
        else //Look forward when no target
        {
            aimDirection = transform.forward.normalized * 10;
        }
        aimingTransform.position = new Vector3(aimDirection.x, aimingTransform.position.y, aimDirection.z);
    }

    void AttackTarget()
    {
        if (CurrentTarget == null) return;
        if (currentWeaponInstances.Count == 0) return;
        var weapon = currentWeaponInstances[usingWeaponIndex % currentWeaponInstances.Count];
        if (weapon.IsAttackable)
        {
            weapon.Attack(currentTarget.transform.position);
            usingWeaponIndex++;
        }
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

    public override void Hit(float damage, HitData hitData)
    {
        SetRecentHitData(hitData);
        CurrentHealth -= damage;
        base.Hit(damage, hitData);
        Debug.Log("Player hit " + damage);
    }

    protected override void SwitchWeapon()
    {
        base.SwitchWeapon();
        UpdateAnimatorWeight();
        UpdateRigWeight();
        currentWeaponSOVariable.Value = CurrentWeaponSO;
        GameEventHandler.Invoke(PlayerWeaponEventCode.OnWeaponChanged, CurrentWeaponSO);
    }

    void UpdateAnimatorWeight()
    {
        if (!animator) return;
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, (int)CurrentWeaponSO.WeaponType + 1 == i ? 1 : 0);
        }
    }

    void UpdateRigWeight()
    {
        for (int i = 0; i < rigs.Count; i++)
        {
            rigs[i].weight = CurrentWeaponSO && (int)CurrentWeaponSO.WeaponType == i ? 1 : 0;
        }
    }

    private void HandleWeaponChanged(object[] parameters)
    {
        if (parameters == null) return;
        if (parameters.Length == 0) return;
        if (parameters[0] is not WeaponSO weaponSO) return;
        CurrentWeaponSO = weaponSO;
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
