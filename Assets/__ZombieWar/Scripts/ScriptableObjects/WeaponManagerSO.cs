using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponManagerSO", menuName = "ZombieWar/ScriptableObjects/WeaponManagerSO")]
public class WeaponManagerSO : ScriptableObject
{
    [SerializeField] List<WeaponSO> weaponSOs;

    public List<WeaponSO> WeaponSOs => weaponSOs;
}
