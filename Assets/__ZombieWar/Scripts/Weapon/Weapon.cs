using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected const string TriggerActionStr = "LaunchProjectile";
    public event Action OnWeaponUsed = delegate { };
    [SerializeField] protected Transform launchPosition;
    [SerializeField] protected AudioSource audioSource;

    #region Variables
    protected float lastUsedTime;
    protected WeaponSO weaponSO;
    protected Character holder;
    ObjectPooling<Projectile> projectilePooling;
    #endregion

    #region Properties
    public WeaponSO WeaponSO => weaponSO;
    protected float weaponDamage => WeaponSO ? WeaponSO.WeaponDamage : 1;
    protected float projectileSpeed => WeaponSO ? WeaponSO.ProjectileSpeed : 50;
    protected float fireRate => WeaponSO ? WeaponSO.FireRate : 1;
    protected float weaponForce => WeaponSO ? WeaponSO.WeaponForce : 1;
    protected Projectile projectilePrefab => WeaponSO ? WeaponSO.ProjectilePrefab : null;
    protected ParticleSystem hitParticlePrefab => WeaponSO ? WeaponSO.HitParticlePrefab : null;
    public virtual bool IsAttackable => lastUsedTime + fireRate <= Time.time;

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

    protected void AddProjectileToPool(Projectile projectile)
    {
        if (!this || projectilePooling == null)
        {
            // Objectpool is null, destroy projectile instead!
            Destroy(projectile.gameObject);
            return;
        }

        projectilePooling.Add(projectile);
    }

    protected virtual void ProjectileCollide(Collision other)
    {
        if (other.rigidbody == null) //Collide with obstacle
        {
            //Do nothing
        }
        else if (other.rigidbody.TryGetComponent(out Character character))
        {
            character.Hit(weaponDamage, new HitData()
            {
                forcePosition = other.GetContact(0).point,
                forcePower = weaponForce,
                forceNormal = other.GetContact(0).normal
            });
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

    public virtual T GetProjectile<T>() where T : Projectile
    {
        return ProjectilePooling?.Get().Cast<T>();
    }

    /// <summary>
    /// Attack at specific direction
    /// </summary>
    /// <param name="target">: Attack direction</param>
    public virtual void Attack(Vector3 target)
    {
        Projectile projectile = GetProjectile();
        if (projectile == null) return;
        projectile.gameObject.SetActive(true);
        projectile.transform.SetPositionAndRotation(launchPosition.position, launchPosition.rotation);
        target.y = launchPosition.position.y;
        var direction = target - launchPosition.position;
        projectile.Launch(direction, projectileSpeed);
        audioSource.PlayOneShot(audioSource.clip);
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
    public virtual void Init(WeaponSO weaponSO, Character holder)
    {
        this.weaponSO = weaponSO;
        this.holder = holder;
    }

    protected virtual void OnDestroy()
    {
        //Dispose object pooling if destroyed
        if (projectilePooling != null)
        {
            projectilePooling.DestroyPool();
            projectilePooling = null;
        }
    }
}
