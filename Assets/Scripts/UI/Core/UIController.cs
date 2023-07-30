using System;
using UnityEngine;

namespace UI.Core
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private WindowDictionary _screens;

        [SerializeField] private GameObject _darkenBackground;

        private void Awake()
        {
            gameObject.SetActive(true);
            foreach (var pair in _screens)
            {
                ShowWindow(pair.Key);
                pair.Value.CloseRequest += HideWindow;
                HideWindow(pair.Value);
            }
        }

        public void ShowWindow(ScreenEnum screen)
        {
            var window = _screens[screen];

            if (window.IsPopup)
            {
                foreach (var pair in _screens)
                {
                    if (pair.Value.IsPopup)
                    {
                        pair.Value.OnClose();
                    }
                }

                _darkenBackground.SetActive(true);
            }
            else
            {
                foreach (var pair in _screens)
                {
                    pair.Value.OnClose();
                }

                _darkenBackground.SetActive(false);
            }

            window.OnOpen();
        }

        public void HideWindow(ScreenEnum screen)
        {
            var window = _screens[screen];
            HideWindow(window);
        }

        private void HideWindow(Window window)
        {
            window.OnClose();
            if (window.IsPopup)
            {
                _darkenBackground.SetActive(false);
            }
        }
    }

    [Serializable]
    public class WindowDictionary : SerializableDictionary<ScreenEnum, Window>
    {
    }
}