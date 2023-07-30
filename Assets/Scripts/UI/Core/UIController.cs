using System;
using UI.Misc;
using UnityEngine;

namespace UI.Core
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private WindowDictionary _screens;

        [SerializeField] private GameObject _darkenBackground;

        private void Awake()
        {
            foreach (var pair in _screens)
            {
                pair.Value.CloseRequest += HideWindow;
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
            window.OnClose();

            if (window.IsPopup)
            {
                _darkenBackground.SetActive(false);
            }
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