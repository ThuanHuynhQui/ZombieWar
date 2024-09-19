using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    protected WeaponSO weaponSO;
    public WeaponSO WeaponSO => weaponSO;
    protected float weaponDamage => weaponSO ? weaponSO.WeaponDamage : 1;
    protected int ammoEachMagazine => weaponSO ? weaponSO.AmmoEachMagazine : 1;
    protected float projectileSpeed => weaponSO ? weaponSO.ProjectileSpeed : 50;
    protected float fireRate => weaponSO ? weaponSO.FireRate : 1;
    protected GameObject projectilePrefab => weaponSO ? weaponSO.ProjectilePrefab : null;

    ObjectPooling<GameObject> projectilePooling;

    protected ObjectPooling<GameObject> ProjectilePooling
    {
        get
        {
            if (projectilePooling == null) InitPooling();
            return projectilePooling;
        }
    }

    void InitPooling()
    {
        if (projectilePrefab == null) return;
        projectilePooling = new ObjectPooling<GameObject>(
                        instantiateMethod: InstantiateMethod,
                        destroyMethod: DestroyMethod,
                        resetMethod: ResetMethod,
                        preAddToPool: PreAddToPool,
                        preLeavePool: PreLeavePool
                    )
        {
            PregenerateOffset = 5
        };

        GameObject InstantiateMethod()
        {
            return Instantiate(projectilePrefab, transform);
        }
        void DestroyMethod(GameObject projectile)
        {
            Destroy(projectile);
        }
        void ResetMethod(GameObject projectile)
        {

        }
        void PreAddToPool(GameObject projectile)
        {
            projectile.SetActive(false);
        }
        void PreLeavePool(GameObject projectile)
        {
            
        }
    }
    /// <summary>
    /// Attack at specific direction
    /// </summary>
    /// <param name="direction">: Attack direction</param>
    public void Attack(Vector3 direction)
    {

    }

    /// <summary>
    /// Attack at forward direction
    /// </summary>
    public void Attack()
    {

    }
}
