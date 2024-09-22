using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ZombieWar/ScriptableObjects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    [Header("Tweak")]
    [SerializeField] protected float weaponDamage;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float weaponRange = 5;
    [SerializeField] protected WeaponType weaponType;
    [Space]
    [Header("References")]
    [SerializeField] protected Sprite weaponIcon;
    [SerializeField] protected Weapon weaponPrefab;
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected ParticleSystem hitParticlePrefab;

    public float WeaponDamage => weaponDamage;
    public float ProjectileSpeed => projectileSpeed;
    public float FireRate => fireRate;
    public float WeaponRange => weaponRange;
    public WeaponType WeaponType => weaponType;
    public Sprite WeaponIcon => weaponIcon;
    public Weapon WeaponPrefab => weaponPrefab;
    public Projectile ProjectilePrefab => projectilePrefab;
    public ParticleSystem HitParticlePrefab => hitParticlePrefab;
}

public enum WeaponType
{
    Rifle = 0,
    Pistol = 1,
    DualPistol = 2
}
