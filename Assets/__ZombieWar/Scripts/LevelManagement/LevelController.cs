using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static event Action<KillAmountChangedEventData> OnKillAmountChanged = delegate { };
    [SerializeField] int killTargetAmount;

    int killedAmount = 0;

    int KilledAmount
    {
        get => killedAmount;
        set
        {
            killedAmount = value;
            OnKillAmountChanged(new(killedAmount, killTargetAmount));
        }
    }
    bool isEnded = false;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(CharacterEventCode.OnCharacterDie, HandleCharacterDie);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(CharacterEventCode.OnCharacterDie, HandleCharacterDie);
    }

    private void Start()
    {
        GameEventHandler.Invoke(LevelEventCode.OnLevelStarted);
        KilledAmount = 0;
    }

    private void HandleCharacterDie(object[] parameters)
    {
        if (parameters == null) return;
        if (parameters.Length == 0) return;
        if (parameters[0] is not Character character) return;
        if (character.CharacterType == CharacterType.Zombie)
        {
            KilledAmount++;
            if (KilledAmount >= killTargetAmount)
            {
                EndLevel(true);
            }
        }
        else if (character.CharacterType == CharacterType.Human) //Player died
        {
            EndLevel(false);
        }
    }

    void EndLevel(bool isVictory)
    {
        if (isEnded) return;
        isEnded = true;
        GameEventHandler.Invoke(LevelEventCode.OnLevelEnded, isVictory);
    }

    public class KillAmountChangedEventData
    {
        public int CurrentAmount;
        public int TotalAmount;

        public KillAmountChangedEventData(int currentAmount, int totalAmount)
        {
            CurrentAmount = currentAmount;
            TotalAmount = totalAmount;
        }
    }
}
