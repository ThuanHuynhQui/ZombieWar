using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSOVariable", menuName = "ZombieWar/ScriptableObjects/WeaponSOVariable")]
public class WeaponSOVariable : ScriptableObject
{
    [SerializeField] protected WeaponSO defaultValue;

    public virtual WeaponSO Value
    {
        get => defaultValue;
        set
        {
            //Do nothing
        }
    }
}
