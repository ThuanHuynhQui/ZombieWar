using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCharacter : Character
{
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

    protected override void Start()
    {
        // base.Start();
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
