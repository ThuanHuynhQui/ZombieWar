using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JoystickDataSO", menuName = "ZombieWar/ScriptableObject/JoystickDataSO")]
public class JoystickDataSO : ScriptableObject
{
    public Vector2 CurrentDirection = new Vector2();
    public float Horizontal => CurrentDirection.x;
    public float Vertical => CurrentDirection.y;
}
