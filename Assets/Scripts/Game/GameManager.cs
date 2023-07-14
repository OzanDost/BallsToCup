using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        public GameState CurrentGameState { get; private set; }

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
        }
    }
}