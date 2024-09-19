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
            joystickDataSO.Value = Vector2.zero;
            return;
        }
        joystickDataSO.Value = joystick.Direction;
    }
}
