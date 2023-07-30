using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        private GameState CurrentGameState { get; set; }

        private void Awake()
        {
            ApplyConfigs();
        }

        private void Start()
        {
            _levelManager.Initialize();

            AddListeners();

            ChangeGameState(GameState.Loading);
        }

        private void AddListeners()
        {
            EventDispatcher.FakeLoadingFinished += OnFakeLoadingFinished;
            EventDispatcher.PlayButtonClicked += OnPlayButtonClicked;
            EventDispatcher.LevelFailed += OnLevelFailed;
            EventDispatcher.LevelSuccess += OnLevelSuccess;
            EventDispatcher.LevelQuitRequested += OnLevelQuitRequested;
            EventDispatcher.LevelRetryRequested += OnLevelRetryRequested;
        }

        private void OnLevelQuitRequested()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnLevelFailed()
        {
            ChangeGameState(GameState.Fail);
        }

        private void OnLevelSuccess()
        {
            ChangeGameState(GameState.Success);
        }

        private void OnFakeLoadingFinished()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnPlayButtonClicked()
        {
            _levelManager.CreateLevel();
            ChangeGameState(GameState.Gameplay);
        }

        private void OnLevelRetryRequested()
        {
            _levelManager.CreateLevel(true);
            ChangeGameState(GameState.Gameplay);
        }

        private void ChangeGameState(GameState newGameState)
        {
            var oldGameState = CurrentGameState;
            CurrentGameState = newGameState;
            EventDispatcher.GameStateChanged?.Invoke(oldGameState, newGameState);
        }

        private void ApplyConfigs()
        {
            RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio;
            Application.targetFrameRate = (int)refreshRate.numerator % 60 == 0 ? 60 : (int)refreshRate.numerator;
            ConfigHelper.Initialize();
        }
    }
}