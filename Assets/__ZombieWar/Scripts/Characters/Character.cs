using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;

public enum CharacterEventCode
{
    /// <summary>
    /// Raised when a character is death.
    /// <para><typeparamref name="Character"/>: Died character</para>
    /// </summary>
    OnCharacterDie,
    /// <summary>
    /// Raised when a character got hit.
    /// <para><typeparamref name="Character"/>: Being hit character</para>
    /// <para><typeparamref name="float"/>: Hit damage</para>
    /// </summary>
    OnCharacterHit,
}

public enum CharacterType
{
    Human,
    Zombie,
    BossZombie
}
public abstract class Character : MonoBehaviour
{
    public event Action OnHit = delegate { };
    public event Action OnDeath = delegate { };
    public event Action<float> OnHealthChanged = delegate { };
    [SerializeField] protected float initialHealth;
    [SerializeField] protected Transform leftHandWeaponContainer;
    [SerializeField] protected Transform rightHandWeaponContainer;
    [SerializeField] protected WeaponSOVariable currentWeaponSOVariable;
    [SerializeField] protected Animator animator;
    [SerializeField] protected RagdollController ragdollController;

    protected virtual CharacterType characterType => CharacterType.Human;
    protected virtual float CurrentHealth
    {
        get => 0;
        set
        {
            OnHealthChanged?.Invoke(CurrentHealth);
        }
    }
    public virtual Character CurrentTarget => null;
    protected WeaponSO currentWeaponSO;
    public virtual WeaponSO CurrentWeaponSO
    {
        get => currentWeaponSO;
        set
        {
            if (currentWeaponSO == value) return;
            currentWeaponSO = value;
            if (!currentWeaponSO) currentWeaponSO = currentWeaponSOVariable.Value; //Set default weapon if null
            SwitchWeapon();
        }
    }
    public float InitialHealth => initialHealth;

    protected List<Weapon> currentWeaponInstances = new();
    protected int usingWeaponIndex = 0;

    protected bool isDeath = false;

    public bool IsDeath => isDeath;
    public Animator Animator => animator;

    protected HitData recentHitData;

    protected virtual void Start()
    {
        CurrentWeaponSO = currentWeaponSOVariable.Value;
        CurrentHealth = initialHealth;
    }

    public virtual void Hit(float damage, HitData hitData)
    {
        OnHealthChanged?.Invoke(CurrentHealth);
        GameEventHandler.Invoke(CharacterEventCode.OnCharacterHit, this);
    }

    protected void SetRecentHitData(HitData recentHitData)
    {
        this.recentHitData = recentHitData;
    }

    public virtual void Die()
    {
        if (isDeath) return;
        isDeath = true;
        GetComponent<CharacterMovement>().enabled = false;
        Animator.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        ragdollController.enabled = true;
        var forceDir = recentHitData.forcePosition - new Vector3(transform.position.x, recentHitData.forcePosition.y, transform.position.z);
        ragdollController.AddForce(forceDir.normalized * recentHitData.forcePower, recentHitData.forcePosition, 0.2f, ForceMode.Impulse);
        StartCoroutine(CR_InvokeEvent());

        IEnumerator CR_InvokeEvent()
        {
            yield return new WaitForSeconds(4f);
            OnDeath?.Invoke();
            GameEventHandler.Invoke(CharacterEventCode.OnCharacterDie, this);
        }
    }

    protected virtual void SwitchWeapon()
    {
        //Destroy current weapons
        foreach (var weaponInstance in currentWeaponInstances)
        {
            Destroy(weaponInstance.gameObject);
        }
        currentWeaponInstances.Clear();
        if (CurrentWeaponSO == null) return;

        //Attach new weapon
        switch (CurrentWeaponSO.WeaponType)
        {
            case WeaponType.DualPistol:
                //Attach right hand
                Weapon instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
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
            default:
                instance = Instantiate(CurrentWeaponSO.WeaponPrefab);
                instance.OnWeaponUsed += () => instance.UpdateUsedtime(Time.time);
                AttachWeapon(rightHandWeaponContainer);
                break;


                void AttachWeapon(Transform container)
                {
                    instance.Init(CurrentWeaponSO, this);
                    instance.transform.SetParent(container);
                    instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    currentWeaponInstances.Add(instance);
                }
        }
        usingWeaponIndex = 0;
    }
}

//Store data character got hit.
public class HitData
{
    public float forcePower;
    public Vector3 forcePosition;
}
