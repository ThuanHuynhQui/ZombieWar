using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;

public enum CharacterEventCode
{
    /// <summary>
    /// Raised when a character is death
    /// <para><typeparamref name="Character"/>: Died character</para>
    /// </summary>
    OnCharacterDie,
    /// <summary>
    /// Raised when a character got hit
    /// <para><typeparamref name="Character"/>: Being hit character</para>
    /// <para><typeparamref name="float"/>: Hit damage</para>
    /// </summary>
    OnCharacterHit,
}
public abstract class Character : MonoBehaviour
{
    public event Action OnHit = delegate { };
    public event Action OnDeath = delegate { };
    public event Action<float> OnHealthChanged = delegate { };
    [SerializeField] protected float initialHealth;
    [SerializeField] protected Transform leftHandWeaponContainer;
    [SerializeField] protected Transform rightHandWeaponContainer;
    [SerializeField] protected WeaponSO defaultWeaponSO;
    [SerializeField] protected Animator animator;

    protected virtual float CurrentHealth { get; set; }
    public virtual Character CurrentTarget => null;
    protected WeaponSO currentWeaponSO;
    public virtual WeaponSO CurrentWeaponSO
    {
        get => currentWeaponSO;
        set
        {
            currentWeaponSO = value;
            if (currentWeaponSO == null) currentWeaponSO = defaultWeaponSO;
            SwitchWeapon();
        }
    }
    public float InitialHealth => initialHealth;

    protected List<Weapon> currentWeaponInstances = new();
    protected int usingWeaponIndex = 0;

    bool isDeath = false;

    public bool IsDeath => isDeath;

    protected virtual void Start()
    {
        CurrentWeaponSO = defaultWeaponSO;
        CurrentHealth = initialHealth;
    }

    public virtual void Hit(float damage)
    {
        OnHealthChanged?.Invoke(CurrentHealth);
        GameEventHandler.Invoke(CharacterEventCode.OnCharacterHit, this);
    }

    public virtual void Die()
    {
        if (isDeath) return;
        isDeath = true;
        OnDeath?.Invoke();
        GameEventHandler.Invoke(CharacterEventCode.OnCharacterDie, this);
    }

    protected virtual void SwitchWeapon()
    {
        //Destroy current weapons
        foreach (var weaponInstance in currentWeaponInstances)
        {
            Destroy(weaponInstance.gameObject);
        }

        if (CurrentWeaponSO == null) return;

        //Attach new weapon
        switch (CurrentWeaponSO.WeaponType)
        {
            case WeaponType.Rifle:
                Weapon instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
                instance.OnWeaponUsed += () => instance.UpdateUsedtime(Time.time);
                AttachWeapon(rightHandWeaponContainer);
                break;
            case WeaponType.Pistol:
                instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
                instance.OnWeaponUsed += () => instance.UpdateUsedtime(Time.time);
                AttachWeapon(leftHandWeaponContainer);
                break;
            case WeaponType.DualPistol:
                //Attach right hand
                instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
                AttachWeapon(rightHandWeaponContainer);
                Weapon leftWeapon = instance;
                //Attach left hand
                instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
                AttachWeapon(leftHandWeaponContainer);
                Weapon rightWeapon = instance;
                //Attach events
                leftWeapon.OnWeaponUsed += () =>
                {
                    rightWeapon.UpdateUsedtime(Time.time);
                    leftWeapon.UpdateUsedtime(Time.time);
                };
                rightWeapon.OnWeaponUsed += () =>
                {
                    rightWeapon.UpdateUsedtime(Time.time);
                    leftWeapon.UpdateUsedtime(Time.time);
                };
                break;

                void AttachWeapon(Transform container)
                {
                    instance.Init(CurrentWeaponSO);
                    instance.transform.SetParent(container);
                    instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    currentWeaponInstances.Add(instance);
                }
        }
        usingWeaponIndex = 0;
    }
}
