using System;
using Data;
using DefaultNamespace;
using Enums;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    public static EventDispatcher Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Action<GameState, GameState> GameStateChanged { get; set; }
    public Action LevelSuccess { get; set; }
    public Action LevelFailed { get; set; }
    public Action LevelQuitRequested { get; set; }
    public Action LevelRetryRequested { get; set; }
    public Action PlayButtonClicked { get; set; }
    public Action FinishButtonClicked { get; set; }
    public Action<LevelData> RequestGameplayInitialize { get; set; }
    public Action<LevelData> GameplayInitialized { get; set; }
    public Action<Ball> BallReleaseRequested { get; set; }
    public Action BallEnteredCup { get; set; }
    public Action BallFellOut { get; set; }
    public Action FakeLoadingFinished { get; set; }
    public Action SufficientBallCountReached { get; set; }
    public Action CameraShakeRequested { get; set; }
}