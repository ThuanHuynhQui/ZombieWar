using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCharacter : Character
{
    [SerializeField] LayerMask playerLayer;
    float currentHealth = 0;
    protected override CharacterType characterType => CharacterType.Zombie;
    protected override float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Max(value, 0);
            base.CurrentHealth = value;
            if (currentHealth <= 0) Die();
        }
    }

    protected float weaponRange => CurrentWeaponSO ? CurrentWeaponSO.WeaponRange : 0;

    private void Update()
    {
        if (isDeath) return;
        if (!IsEnoughRange()) return;
        AttackPlayer();
    }

    void AttackPlayer()
    {
        if (currentWeaponInstances.Count == 0) return;
        var weapon = currentWeaponInstances[usingWeaponIndex % currentWeaponInstances.Count];
        if (weapon.IsAttackable)
        {
            weapon.Attack();
        }
    }

    bool IsEnoughRange()
    {
        Ray ray = new(transform.position, transform.forward);
        if (Physics.Raycast(ray, weaponRange, playerLayer))
        {
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        animator.SetFloat("AnimationSpeedMultiplier", GetComponent<CharacterMovement>().Speed);
    }

    public void ResetToOrgState()
    {
        CurrentHealth = initialHealth;
        Animator.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CharacterMovement>().enabled = true;
        GetComponent<Collider>().enabled = true;
        ragdollController.enabled = false;
        isDeath = false;
    }

    public override void Hit(float damage, HitData hitData)
    {
        SetRecentHitData(hitData);
        CurrentHealth -= damage;
        base.Hit(damage, hitData);
    }
}
