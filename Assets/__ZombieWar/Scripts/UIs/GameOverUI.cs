using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Template.Events;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject failedUI;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button restartBtn;
    private void Awake()
    {
        GameEventHandler.AddActionEvent(LevelEventCode.OnLevelEnded, HandleLevelEnded);
        GameEventHandler.AddActionEvent(LevelEventCode.OnLevelStarted, HandleLevelStarted);
        restartBtn.onClick.AddListener(HandleRestartBtnClicked);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(LevelEventCode.OnLevelEnded, HandleLevelEnded);
        GameEventHandler.RemoveActionEvent(LevelEventCode.OnLevelStarted, HandleLevelStarted);
        restartBtn.onClick.RemoveListener(HandleRestartBtnClicked);
    }

    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void HandleLevelEnded(object[] parameters)
    {
        if (parameters == null) return;
        if (parameters.Length == 0) return;
        if (parameters[0] is not bool isVictory) return;
        victoryUI.SetActive(isVictory);
        failedUI.SetActive(!isVictory);
        canvasGroup.DOFade(1, 0.5f).OnComplete(() => canvasGroup.blocksRaycasts = true);
    }

    private void HandleRestartBtnClicked()
    {
        GameEventHandler.Invoke(LevelEventCode.OnLevelRestart);
    }

    private void HandleLevelStarted()
    {
        canvasGroup.blocksRaycasts = false;
        if (canvasGroup.alpha == 0) return;
        canvasGroup.DOFade(0, 0.5f);
    }
}
