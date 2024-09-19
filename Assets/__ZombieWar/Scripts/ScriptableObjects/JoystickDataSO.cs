using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JoystickDataSO", menuName = "ZombieWar/ScriptableObjects/JoystickDataSO")]
public class JoystickDataSO : Vector2VariableSO
{
    public float Horizontal => Value.x;
    public float Vertical => Value.y;
}
