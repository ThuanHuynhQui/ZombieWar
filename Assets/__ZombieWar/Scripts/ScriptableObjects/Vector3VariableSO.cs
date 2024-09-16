using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector3VariableSO", menuName = "ScriptableObjects/Variables/Vector 3")]
public class Vector3VariableSO : ScriptableObject
{
    public Vector3 Value = new();
}
