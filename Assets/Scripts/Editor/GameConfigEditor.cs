using Data;
using Utils;
using UnityEditor;
using UnityEngine;

namespace Editor
{
       // This class represents a custom editor window for the GameConfig object.
   public class GameConfigEditor : EditorWindow
    {
        // The GameConfig object that will be edited in this window.
        private GameConfig _gameConfig;

        // This method is used to show the editor window.
        [MenuItem("Tools/GameConfig Editor")]
        public static void ShowWindow()
        {
            // Get the existing open window or if none, make a new one
            GetWindow<GameConfigEditor>();
        }

        // This method is called when the window becomes enabled and active.
        private void OnEnable()
        {
            // Load the GameConfig object from the Resources folder.
            _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
        }

        // This method is called for rendering and handling GUI events.
        private void OnGUI()
        {
            // If the GameConfig object is not found, display an error message.
            if (_gameConfig == null)
            {
                EditorGUILayout.LabelField("GameConfig not found at path: " + PathHelper.GameConfigPath);
                return;
            }

            // Begin a group of controls that trigger a change check.
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

            // Display the properties in the editor window.
            EditorGUILayout.PropertyField(rotationSpeedProp);
            EditorGUILayout.PropertyField(centerDistanceSensitivityProp);
            EditorGUILayout.PropertyField(minInputThresholdProp);

            EditorGUILayout.LabelField("Ball Settings", titleStyle);

            PhysicMaterial physicMaterial = (PhysicMaterial)ballPhysicMaterialProp.objectReferenceValue;
            if (physicMaterial != null)
            {
                UnityEditor.Editor physicMaterialEditor = UnityEditor.Editor.CreateEditor(physicMaterial);
                physicMaterialEditor.OnInspectorGUI();
            }

            // Display the materials in the editor window.
            for (int i = 0; i < _gameConfig.BallMaterials.Count; i++)
            {
                Material material = _gameConfig.BallMaterials[i];
                if (material != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Material " + i, material, typeof(Material), false);
                    MaterialEditor materialEditor = (MaterialEditor)UnityEditor.Editor.CreateEditor(material);
                    materialEditor.OnPreviewGUI(GUILayoutUtility.GetRect(50, 50), EditorStyles.helpBox);
                    EditorGUILayout.EndHorizontal();
                }
            }

            // Display the Rigidbody property in the editor window.
            EditorGUILayout.PropertyField(ballRigidbodyProp);


            Rigidbody rigidbody = (Rigidbody)ballRigidbodyProp.objectReferenceValue;
            if (rigidbody != null)
            {
                UnityEditor.Editor rigidbodyEditor = UnityEditor.Editor.CreateEditor(rigidbody);
                rigidbodyEditor.OnInspectorGUI();
            }

            // If any changes have been made, apply them to the serialized object.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}