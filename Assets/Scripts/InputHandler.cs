using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magiclab.Utility.GenericUtilities
{
    public class InputHandler : MonoBehaviour
    {
        public bool useCustomDPI = true;
        public float customDPI = 326f;
        public static float ScreenDPI { get; private set; }
        public static Vector2 DeltaMousePosition { get; private set; }
        private static InputHandler _instance;

        private static Vector3 _lastMousePosition;
        private static Vector3 _firstMousePosition;
        private static Vector3 _lastRayWorldPosition;
        private static Vector3 _firstRayWorldPosition;
        private static bool _isActiveDeltaScreenPositionWithRay;
        private static float _mouseDownTime;
        private static float _dragTime;
        private float _currentMaxDistance;
        

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            ScreenDPI = GetScreenDPI();

            SetGameCamera(Camera.main);
        }

        private void Update()
        {
            if (GetMouseButtonDown())
            {
                _firstMousePosition = GetMousePosition();
                _lastMousePosition = _firstMousePosition;
                DeltaMousePosition = Vector2.zero;
                _mouseDownTime = Time.time;
            }
            else if (GetMouseButton() || GetMouseButtonUp())
            {
                var mousePosition = GetMousePosition();
                DeltaMousePosition = mousePosition - _lastMousePosition;
                _lastMousePosition = mousePosition;
                _dragTime = Time.time;

                if (GetMouseButtonUp())
                {
                    _isActiveDeltaScreenPositionWithRay = false;
                }
            }
            else
            {
                DeltaMousePosition = Vector2.zero;
            }
        }

        public static bool GetMouseButtonDown()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public static bool GetMouseButton()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;
            
            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase != TouchPhase.Ended && touchPhase != TouchPhase.Canceled;
#else
            return Input.GetMouseButton(0);
#endif
        }

        public static bool GetMouseButtonUp()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;

            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }

        public static Vector3 GetMousePosition()
        {
            var mousePosition = Vector3.zero;

            if (GetMouseButton() || GetMouseButtonDown() || GetMouseButtonUp())
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                mousePosition = Input.GetTouch(0).position;
#else
                mousePosition = Input.mousePosition;
#endif
            }

            return mousePosition;
        }

        public static Vector2 GetMousePositionVector2()
        {
            var mousePosition = Vector3.zero;

            if (GetMouseButton() || GetMouseButtonDown() || GetMouseButtonUp())
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                mousePosition = Input.GetTouch(0).position;
#else
                mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
            }

            return mousePosition;
        }

        
        public static void SetGameCamera(Camera gameCamera)
        {
            if (gameCamera == null)
                return;

        }

        public float GetScreenDPI()
        {
            var dpi = Screen.dpi;
#if UNITY_EDITOR
            if (useCustomDPI)
            {
                dpi = customDPI;
            }
#endif
            return dpi;
        }
    }

    public enum SwipeDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
}