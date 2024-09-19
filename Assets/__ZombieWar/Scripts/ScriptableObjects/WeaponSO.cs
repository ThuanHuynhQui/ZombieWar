using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ZombieWar/ScriptableObjects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    [Header("Tweak")]
    [SerializeField] protected float weaponDamage;
    [SerializeField] protected int ammoEachMagazine;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float fireRate;
    [Space]
    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected BaseWeapon weaponPrefab;
    [SerializeField] protected GameObject projectilePrefab;

    public float WeaponDamage => weaponDamage;
    public int AmmoEachMagazine => ammoEachMagazine;
    public float ProjectileSpeed => projectileSpeed;
    public float FireRate => fireRate;

    public Animator Animator => animator;
    public BaseWeapon WeaponPrefab => weaponPrefab;
    public GameObject ProjectilePrefab => projectilePrefab;
}
