using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected const string TriggerActionStr = "LaunchProjectile";
    public event Action OnWeaponUsed = delegate { };
    [SerializeField] protected Transform launchPosition;

    #region Variables
    protected float lastUsedTime;
    protected WeaponSO weaponSO;
    ObjectPooling<Projectile> projectilePooling;
    // ObjectPooling<ParticleSystem> hitParticlePooling;
    #endregion

    #region Properties
    public WeaponSO WeaponSO => weaponSO;
    protected float weaponDamage => WeaponSO ? WeaponSO.WeaponDamage : 1;
    protected float projectileSpeed => WeaponSO ? WeaponSO.ProjectileSpeed : 50;
    protected float fireRate => WeaponSO ? WeaponSO.FireRate : 1;
    protected Projectile projectilePrefab => WeaponSO ? WeaponSO.ProjectilePrefab : null;
    protected ParticleSystem hitParticlePrefab => WeaponSO ? WeaponSO.HitParticlePrefab : null;
    public bool IsAttackable => lastUsedTime + fireRate <= Time.time;

    // protected ObjectPooling<ParticleSystem> HitParticlePooling
    // {
    //     get
    //     {
    //         if (hitParticlePooling == null) InitHitParticlePooling();
    //         return hitParticlePooling;
    //     }
    // }

    protected ObjectPooling<Projectile> ProjectilePooling
    {
        get
        {
            if (projectilePooling == null) InitProjectilePooling();
            return projectilePooling;
        }
    }
    #endregion

    void InitProjectilePooling()
    {
        if (projectilePrefab == null) return;
        projectilePooling = new ObjectPooling<Projectile>(
                        instantiateMethod: InstantiateMethod,
                        destroyMethod: DestroyMethod,
                        resetMethod: ResetMethod,
                        preAddToPool: PreAddToPool,
                        preLeavePool: PreLeavePool
                    )
        {
            PregenerateOffset = 5
        };

        Projectile InstantiateMethod()
        {
            Projectile instance = Instantiate(projectilePrefab, transform);
            instance.Setup(
                returnMethod: AddProjectileToPool,
                collideMethod: ProjectileCollide
                );
            return instance;
        }
        void DestroyMethod(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }
        void ResetMethod(Projectile projectile)
        {
            projectile.ResetToOrgState();
        }
        void PreAddToPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            projectile.transform.parent = transform;
        }
        void PreLeavePool(Projectile projectile)
        {

        }
    }

    // void InitHitParticlePooling()
    // {
    //     if (hitParticlePrefab == null) return;
    //     hitParticlePooling = new ObjectPooling<ParticleSystem>(
    //                     instantiateMethod: InstantiateMethod,
    //                     destroyMethod: DestroyMethod,
    //                     resetMethod: ResetMethod,
    //                     preAddToPool: PreAddToPool,
    //                     preLeavePool: PreLeavePool
    //                 )
    //     {
    //         PregenerateOffset = 5
    //     };

    //     ParticleSystem InstantiateMethod()
    //     {
    //         ParticleSystem instance = Instantiate(hitParticlePrefab, transform);
    //         return instance;
    //     }
    //     void DestroyMethod(ParticleSystem ps)
    //     {
    //         Destroy(ps.gameObject);
    //     }
    //     void ResetMethod(ParticleSystem ps)
    //     {
    //         ps.Clear();
    //     }
    //     void PreAddToPool(ParticleSystem ps)
    //     {
    //         ps.Stop();
    //         ps.transform.parent = transform;
    //     }
    //     void PreLeavePool(ParticleSystem ps)
    //     {

    //     }
    // }

    protected void AddProjectileToPool(Projectile projectile)
    {
        if (projectilePooling == null)
        {
            Debug.LogWarning("Objectpool is null, destroy projectile instead!");
            Destroy(projectile.gameObject);
            return;
        }

        projectilePooling.Add(projectile);
    }

    protected void ProjectileCollide(Collision other)
    {
        if (other.rigidbody == null) //Collide with obstacle
        {
            //Do nothing
        }
        else if (other.rigidbody.TryGetComponent(out Character character))
        {
            character.Hit(weaponDamage);
        }

        //Spawn hit particle at contact point
        Instantiate(hitParticlePrefab, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal));
    }

    /// <summary>
    /// Get projectile method
    /// </summary>
    /// <returns></returns>
    protected virtual Projectile GetProjectile()
    {
        return ProjectilePooling?.Get();
    }

    /// <summary>
    /// Attack at specific direction
    /// </summary>
    /// <param name="direction">: Attack direction</param>
    public virtual void Attack(Vector3 direction)
    {
        Projectile projectile = GetProjectile();
        if (projectile == null) return;
        projectile.gameObject.SetActive(true);
        projectile.transform.SetPositionAndRotation(launchPosition.position, launchPosition.rotation);
        projectile.Launch(direction, projectileSpeed);
        OnWeaponUsed?.Invoke();
    }

    /// <summary>
    /// Attack at forward direction
    /// </summary>
    public virtual void Attack()
    {
        Attack(launchPosition.forward);
    }

    public void UpdateUsedtime(float value)
    {
        lastUsedTime = value;
    }
    public void Init(WeaponSO weaponSO)
    {
        this.weaponSO = weaponSO;
    }
}
