using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector2VariableSO", menuName = "ScriptableObjects/Variables/Vector 2")]
public class Vector2VariableSO : ScriptableObject
{
    public Vector2 Value = new();
}
