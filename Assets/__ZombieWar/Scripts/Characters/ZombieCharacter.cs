using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCharacter : Character
{
    float currentHealth = 0;

    protected override float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Max(value, 0);
            if (currentHealth <= 0) Die();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        animator.SetFloat("AnimationSpeedMultiplier", GetComponent<CharacterMovement>().Speed);
    }

    public void ResetToOrgState()
    {
        currentHealth = initialHealth;
    }

    public override void Hit(float damage)
    {
        CurrentHealth -= damage;
        base.Hit(damage);
    }

    public override void Die()
    {
        base.Die();
        gameObject.SetActive(false);
    }
}
