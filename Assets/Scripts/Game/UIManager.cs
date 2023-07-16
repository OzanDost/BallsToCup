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

            if (newState == GameState.Success)
            {
                _uiFrame.OpenWindow(ScreenIds.SuccessPopup);
            }

            if (newState == GameState.Fail)
            {
                _uiFrame.OpenWindow(ScreenIds.FailPopup);
            }
        }
    }
}