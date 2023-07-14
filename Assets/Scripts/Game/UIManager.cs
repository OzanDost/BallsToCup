using System;
using Enums;
using ThirdParty;
using ThirdParty.uiframework;
using UI.Popups;
using UnityEngine;
using Utils;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UISettings _defaultUISettings;

        private UIFrame _uiFrame;

        private void Awake()
        {
            _uiFrame = _defaultUISettings.CreateUIInstance();
            AddListeners();
        }

        private void AddListeners()
        {
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
            Signals.Get<LevelFinished>().AddListener(OnLevelFinished);
        }

        private void OnLevelFinished(bool isSuccess)
        {
            _uiFrame.OpenWindow(isSuccess ? ScreenIds.SuccessPopup : ScreenIds.FakeRewardedPopup);
        }


        private void OnRewardedPopupRequested(Action successActionCallBack, Action failedActionCallBack)
        {
            _uiFrame.OpenWindow(ScreenIds.FakeRewardedPopup,
                new FakeRewardedPopupProperties(successActionCallBack, failedActionCallBack));
        }


        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState == GameState.Loading)
            {
                _uiFrame.OpenWindow(ScreenIds.LoadingWindow);
            }

            if (newState == GameState.Menu)
            {
                _uiFrame.OpenWindow(ScreenIds.MenuWindow);
            }

            if (newState == GameState.Gameplay)
            {
                _uiFrame.OpenWindow(ScreenIds.GameplayWindow);
            }
        }
    }
}