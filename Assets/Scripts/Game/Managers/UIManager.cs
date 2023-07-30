using Enums;
using UI.Core;
using UnityEngine;

namespace Game.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIController _uiController;

        private void Awake()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState == GameState.Loading)
            {
                _uiController.ShowWindow(ScreenEnum.LoadingWindow);
            }

            if (newState == GameState.Menu)
            {
                _uiController.ShowWindow(ScreenEnum.MainMenuWindow);
            }

            if (newState == GameState.Gameplay)
            {
                _uiController.ShowWindow(ScreenEnum.GameWindow);
            }

            if (newState == GameState.Success)
            {
                _uiController.ShowWindow(ScreenEnum.SuccessPopup);
            }

            if (newState == GameState.Fail)
            {
                _uiController.ShowWindow(ScreenEnum.FailPopup);
            }
        }
    }
}