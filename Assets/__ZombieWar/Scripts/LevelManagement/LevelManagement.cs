using System;
using System.Collections;
using System.Collections.Generic;
using Template.Events;
using UnityEngine;

public enum LevelEventCode
{
    /// <summary>
    /// Raised when player click restart the game.
    /// </summary>
    OnLevelRestart,
    /// <summary>
    /// Raised when level is ended.
    /// <para><typeparamref name="bool"/>: is victory</para>
    /// </summary>
    OnLevelEnded,
    /// <summary>
    /// Raised when level is started
    /// </summary>
    OnLevelStarted
}

public class LevelManagement : MonoBehaviour
{
    [SerializeField] GameObject levelPrefab;

    GameObject levelInstance;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(LevelEventCode.OnLevelRestart, HandleLevelRestart);
    }
    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(LevelEventCode.OnLevelRestart, HandleLevelRestart);
    }

    private void Start()
    {
        SpawnNewLevel();
    }

    void SpawnNewLevel()
    {
        if (levelInstance) Destroy(levelInstance);
        levelInstance = Instantiate(levelPrefab);
    }

    private void HandleLevelRestart()
    {
        SpawnNewLevel();
    }
}
