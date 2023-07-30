using System;
using UnityEngine;

namespace UI.Core
{
    public class Window : MonoBehaviour
    {
        public Action<Window> CloseRequest { get; set; }

        public bool IsVisible { get; private set; }
        public virtual bool IsPopup => false;

        public virtual void OnClose()
        {
            if (!IsVisible)
            {
                Debug.LogWarning($"The window {gameObject.name} is not open. It cannot be closed.");
            }

            gameObject.SetActive(false);
            IsVisible = false;
        }

        public virtual void OnOpen()
        {
            if (IsVisible)
            {
                Debug.LogWarning($"The window {gameObject.name} is already open. It cannot be opened again.");
            }

            gameObject.SetActive(true);
            IsVisible = true;
        }
    }
}