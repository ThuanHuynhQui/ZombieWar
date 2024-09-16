using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickDataGetter : MonoBehaviour
{
    [SerializeField] Joystick joystick;
    [SerializeField] JoystickDataSO joystickDataSO;

    private void FixedUpdate()
    {
        if (joystickDataSO == null)
        {
            joystickDataSO.CurrentDirection = Vector2.zero;
            return;
        }
        joystickDataSO.CurrentDirection = joystick.Direction;
    }
}
