using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] JoystickDataSO joystickDataSO;
    [SerializeField] CharacterMovement characterMovement;

    private void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(joystickDataSO.CurrentDirection.x, 0, joystickDataSO.CurrentDirection.y);
        characterMovement.MoveDirection = moveDir;
    }
}
