using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TubeSelectorWindow : EditorWindow
    {
        public Action<GameObject> OnTubeSelected;

        private GameObject[] _tubes;

        [MenuItem("Window/Tube Selector")]
        public static void ShowWindow()
        {
            GetWindow<TubeSelectorWindow>("Tube Selector");
        }

        private void OnEnable()
        {
            // Load all GameObjects from the Tubes directory
            _tubes = Resources.LoadAll<GameObject>("Prefabs/Game/Tubes");
        }

        private void OnGUI()
        {
            foreach (var tube in _tubes)
            {
                if (GUILayout.Button(tube.name))
                {
                    OnTubeSelected?.Invoke(tube);
                    Close();
                }
            }
        }
    }

}