using DefaultNamespace;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
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
            Signals.Get<FakeLoadingFinished>().AddListener(OnFakeLoadingFinished);
            Signals.Get<PlayButtonClicked>().AddListener(OnPlayButtonClicked);
            Signals.Get<LevelFailed>().AddListener(OnLevelFailed);
            Signals.Get<LevelSuccess>().AddListener(OnLevelSuccess);
            Signals.Get<LevelQuitRequested>().AddListener(OnLevelQuitRequested);
            Signals.Get<LevelRetryRequested>().AddListener(OnLevelRetryRequested);
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
            Signals.Get<GameStateChanged>().Dispatch(oldGameState, newGameState);
        }

        private void ApplyConfigs()
        {
            RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio;
            Application.targetFrameRate = (int)refreshRate.numerator % 60 == 0 ? 60 : (int)refreshRate.numerator;
            ConfigHelper.Initialize();
        }
    }
}