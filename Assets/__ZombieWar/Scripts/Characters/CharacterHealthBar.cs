using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] SlicedFilledImage filler;
    void Awake()
    {
        character.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        character.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float currentHealth)
    {
        filler.fillAmount = currentHealth / character.InitialHealth;
    }
}
