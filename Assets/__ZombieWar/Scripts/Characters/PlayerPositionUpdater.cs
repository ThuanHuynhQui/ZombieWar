using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionUpdater : MonoBehaviour
{
    [SerializeField] Vector3VariableSO currentPlayerPositionSO;

    // Update is called once per frame
    void Update()
    {
        currentPlayerPositionSO.Value = transform.position;
    }
}
