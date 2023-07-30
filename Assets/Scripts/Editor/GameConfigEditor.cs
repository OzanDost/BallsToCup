using Data;
using Utils;

namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    public class GameConfigEditor : EditorWindow
    {
        private GameConfig _gameConfig;

        [MenuItem("Tools/Game Config Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameConfigEditor>();
        }

        private void OnEnable()
        {
            _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
        }

        private void OnGUI()
        {
            if (_gameConfig == null)
            {
                EditorGUILayout.LabelField("GameConfig not found at path: " + PathHelper.GameConfigPath);
                return;
            }

            EditorGUI.BeginChangeCheck();

            // Draw GUI controls for each property of GameConfig
            GUIStyle titleStyle = new GUIStyle()
                { fontSize = 14, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            EditorGUILayout.LabelField("Input Settings", titleStyle);
            SerializedObject serializedObject = new SerializedObject(_gameConfig);
            SerializedProperty rotationSpeedProp = serializedObject.FindProperty("_rotationSpeed");
            SerializedProperty centerDistanceSensitivityProp =
                serializedObject.FindProperty("_centerDistanceSensitivity");
            SerializedProperty minInputThresholdProp = serializedObject.FindProperty("_minInputThreshold");

            SerializedProperty ballPhysicMaterialProp = serializedObject.FindProperty("_ballPhysicMaterial");
            SerializedProperty ballRigidbodyProp = serializedObject.FindProperty("_ballRigidbody");

            EditorGUILayout.PropertyField(rotationSpeedProp);
            EditorGUILayout.PropertyField(centerDistanceSensitivityProp);
            EditorGUILayout.PropertyField(minInputThresholdProp);

            EditorGUILayout.LabelField("Ball Settings", titleStyle);

            PhysicMaterial physicMaterial = (PhysicMaterial)ballPhysicMaterialProp.objectReferenceValue;
            if (physicMaterial != null)
            {
                Editor physicMaterialEditor = Editor.CreateEditor(physicMaterial);
                physicMaterialEditor.OnInspectorGUI();
            }

            for (int i = 0; i < _gameConfig.BallMaterials.Count; i++)
            {
                Material material = _gameConfig.BallMaterials[i];
                if (material != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Material " + i, material, typeof(Material), false);
                    MaterialEditor materialEditor = (MaterialEditor)Editor.CreateEditor(material);
                    materialEditor.OnPreviewGUI(GUILayoutUtility.GetRect(50, 50), EditorStyles.helpBox);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.PropertyField(ballRigidbodyProp);


            Rigidbody rigidbody = (Rigidbody)ballRigidbodyProp.objectReferenceValue;
            if (rigidbody != null)
            {
                Editor rigidbodyEditor = Editor.CreateEditor(rigidbody);
                rigidbodyEditor.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}