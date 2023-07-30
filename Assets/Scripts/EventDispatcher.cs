using System;
using Data;
using DefaultNamespace;
using Enums;
using UnityEngine;

public static class EventDispatcher 
{

    public static Action<GameState, GameState> GameStateChanged { get; set; }
    public static Action LevelSuccess { get; set; }
    public static Action LevelFailed { get; set; }
    public static Action LevelQuitRequested { get; set; }
    public static Action LevelRetryRequested { get; set; }
    public static Action PlayButtonClicked { get; set; }
    public static Action FinishButtonClicked { get; set; }
    public static Action<LevelData> RequestGameplayInitialize { get; set; }
    public static Action<LevelData> GameplayInitialized { get; set; }
    public static Action<Ball> BallReleaseRequested { get; set; }
    public static Action BallEnteredCup { get; set; }
    public static Action BallFellOut { get; set; }
    public static Action FakeLoadingFinished { get; set; }
    public static Action SufficientBallCountReached { get; set; }
    public static Action CameraShakeRequested { get; set; }
}