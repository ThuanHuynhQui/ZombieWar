using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    [SerializeField] Image outline;
    [SerializeField] Image weaponImageUI;
    [SerializeField] Button button;
    WeaponSO weaponSO;
    public WeaponSO WeaponSO
    {
        get => weaponSO;
        set
        {
            weaponSO = value;
            UpdateView();
        }
    }

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PlayerWeaponEventCode.OnWeaponChanged, HandleWeaponChanged);
        button.onClick.AddListener(HandleButtonClicked);

    }

    //Change outline color to indicate which weapon is selected
    private void HandleWeaponChanged(object[] parameters)
    {
        if (!WeaponSO) return;
        if (parameters == null) return;
        if (parameters.Length == 0) return;
        if (parameters[0] is not WeaponSO weaponSO) return;
        if (!weaponSO) return;
        outline.color = weaponSO == WeaponSO ? Color.green : Color.white;
    }

    private void HandleButtonClicked()
    {
        if (!weaponSO) return;
        GameEventHandler.Invoke(PlayerWeaponEventCode.OnRequestWeaponChange, WeaponSO);
    }

    private void UpdateView()
    {
        weaponImageUI.sprite = WeaponSO ? WeaponSO.WeaponIcon : null;
    }
}
