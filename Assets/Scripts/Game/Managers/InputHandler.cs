using UnityEngine;

namespace Game.Managers
{
    public class InputHandler : MonoBehaviour
    {
        private float _currentMaxDistance;
        
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
    }
}