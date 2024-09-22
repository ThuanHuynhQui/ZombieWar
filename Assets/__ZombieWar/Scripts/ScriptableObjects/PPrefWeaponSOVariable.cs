using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefWeaponSO", menuName = "ZombieWar/ScriptableObjects/PPrefWeaponSO")]
public class PPrefWeaponSOVariable : WeaponSOVariable
{
    [SerializeField] string key;
    [SerializeField] WeaponManagerSO weaponManagerSO;
    WeaponSO value = null;
    public override WeaponSO Value
    {
        get
        {
            string weaponName = PlayerPrefs.GetString(key, defaultValue.name);
            value = weaponManagerSO.WeaponSOs.Find(weapon => weapon.name.Equals(weaponName));
            return value;
        }
        set
        {
            if (value != null)
            {
                PlayerPrefs.SetString(key, value.name);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(key))
        {
            if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long localID)){
                key = name + "_" + guid;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}
