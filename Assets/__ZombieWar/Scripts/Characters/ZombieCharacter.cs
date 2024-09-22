using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ZombieCharacter : Character
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Renderer modelRenderer;
    float currentHealth = 0;
    public override CharacterType CharacterType => CharacterType.Zombie;
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
        characterMovement.IsStop = false;
        Animator.enabled = true;
        GetComponent<CharacterMovement>().enabled = true;
        GetComponent<Collider>().enabled = true;
        ragdollController.enabled = false;
        isDeath = false;
        modelRenderer.material.SetFloat("_Dissolve", 0);
    }

    public override void Hit(float damage, HitData hitData)
    {
        SetRecentHitData(hitData);
        CurrentHealth -= damage;
        base.Hit(damage, hitData);
    }

    public override void Die()
    {
        base.Die();
        modelRenderer.material.DOFloat(1, "_Dissolve", 0.5f);
    }
}
