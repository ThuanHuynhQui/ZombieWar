using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI remainingAmountTxt;
    private void Awake()
    {
        LevelController.OnKillAmountChanged += HandleKillAmountChanged;
    }

    private void OnDestroy()
    {
        LevelController.OnKillAmountChanged -= HandleKillAmountChanged;
    }

    private void HandleKillAmountChanged(LevelController.KillAmountChangedEventData data)
    {
        remainingAmountTxt.text = (Mathf.Max(data.TotalAmount - data.CurrentAmount, 0)).ToString();
    }
}
