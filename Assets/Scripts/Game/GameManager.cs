using System;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;

        private GameState _currentGameState;

        private void Awake()
        {
            ApplyConfigs();

            _levelManager.Initalize();
        }
        private void ChangeGameState(GameState newGameState)
        {
            Signals.Get<GameStateChanged>().Dispatch(_currentGameState, newGameState);
            _currentGameState = newGameState;
        }

        private void ApplyConfigs()
        {
            RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio;
            Application.targetFrameRate = (int)refreshRate.numerator % 60 == 0 ? 60 : (int)refreshRate.numerator;
        }
    }
}