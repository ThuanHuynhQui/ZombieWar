using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefIntVariableSO", menuName = "ScriptableObjects/PPrefs/Int")]
public class PPrefIntVariableSO : IntVariableSO
{
    [SerializeField] protected string key;
    public override int Value
    {
        get => PlayerPrefs.GetInt(key, runtimeValue);
        set => PlayerPrefs.SetInt(key, value);
    }
}
