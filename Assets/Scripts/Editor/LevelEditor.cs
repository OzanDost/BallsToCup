using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DefaultNamespace;
using Game;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Editor
{
    public class LevelEditor : EditorWindow
    {
        [EnumToggleButtons]
        [OnValueChanged("OnModeChanged")]
        [SerializeField] private LevelEditorMode _mode;

        [ShowIf("@_mode == LevelEditorMode.Edit")]
        [LabelText("Level")]
        [AssetSelector(Paths = PathHelper.LevelResourcesPath)]
        [OnValueChanged("OnLoadedLevelChanged")]
        [SerializeField] private LevelData _loadedLevelData;

        [HideLabel]
        [Title("Level Data")]
        [HorizontalGroup("row")]
        [SerializeField] private LevelDataEditorWrapper _levelDataEditorWrapper = new();

        private GameConfig _gameConfig;


        private Ball _ballPrefab;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>();
        }

        private void OnGUI()
        {
            // EnumToggleButtons equivalent
            _mode = (LevelEditorMode)GUILayout.Toolbar((int)_mode, Enum.GetNames(typeof(LevelEditorMode)));

            // ShowIf equivalent
            if (_mode == LevelEditorMode.Edit)
            {
                EditorGUILayout.LabelField("Level");
                // AssetSelector equivalent
                _loadedLevelData = (LevelData)EditorGUILayout.ObjectField(_loadedLevelData, typeof(LevelData), false);

                if (_loadedLevelData != null)
                {
                    _levelDataEditorWrapper.SetFields(_loadedLevelData);
                    
                    // Display fields from LevelDataEditorWrapper
                    _levelDataEditorWrapper.LevelName =
                        EditorGUILayout.TextField("Level Name", _levelDataEditorWrapper.LevelName);
                    _levelDataEditorWrapper.LevelId = EditorGUILayout.IntField("Level ID", _levelDataEditorWrapper.LevelId);
                    _levelDataEditorWrapper.BallCount =
                        EditorGUILayout.IntField("Ball Count", _levelDataEditorWrapper.BallCount);
                    _levelDataEditorWrapper.BallTargetCount =
                        EditorGUILayout.IntField("Ball Target Count", _levelDataEditorWrapper.BallTargetCount);
                    _levelDataEditorWrapper.Tube =
                        (GameObject)EditorGUILayout.ObjectField("Tube", _levelDataEditorWrapper.Tube, typeof(GameObject),
                            false);
                    
                    //Draw the Asset Preview of the Tube
                    if (_levelDataEditorWrapper.Tube != null)
                    {
                        Texture2D previewImage = AssetPreview.GetAssetPreview(_levelDataEditorWrapper.Tube);
                        if (previewImage != null)
                        {
                            GUILayout.Label(previewImage);
                        }
                    }
                }
            }

            
            

            if (GUILayout.Button("Save Level"))
            {
                SaveLevel();
            }

            if (GUILayout.Button("Create Level"))
            {
                CreateLevel();
            }

            if (GUILayout.Button("Populate Balls"))
            {
                PopulateBalls();
            }
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.EnteredPlayMode)
            {
                var tubesInScene = FindObjectsOfType<Tube>();
                foreach (var tube in tubesInScene)
                {
                    DestroyImmediate(tube.gameObject);
                }

                EditorUtility.ClearProgressBar();
                GetWindow<LevelEditor>().Close();
            }
        }

        private void OnModeChanged()
        {
            _levelDataEditorWrapper = new();
            _loadedLevelData = null;
        }

        private void OnLoadedLevelChanged()
        {
            if (_loadedLevelData == null) return;
            _levelDataEditorWrapper.SetFields(_loadedLevelData);
        }


        [Button]
        [ShowIf("@_mode == LevelEditorMode.Edit")]
        private void SaveLevel()
        {
            if (!RaiseWarning("You are about to overwrite an existing LevelData. Are you sure?"))
            {
                return;
            }

            if (!_loadedLevelData.name.Equals(_levelDataEditorWrapper.LevelName) &&
                !_levelDataEditorWrapper.LevelName.IsNullOrWhitespace())
            {
                var path = AssetDatabase.GetAssetPath(_loadedLevelData);
                AssetDatabase.RenameAsset(path, _levelDataEditorWrapper.LevelName);
            }

            var tube = _levelDataEditorWrapper.Tube.GetComponent<Tube>();
            _loadedLevelData.SetFields(_levelDataEditorWrapper.LevelId, _levelDataEditorWrapper.BallCount,
                _levelDataEditorWrapper.BallTargetCount, tube);

            EditorUtility.SetDirty(_loadedLevelData);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(_loadedLevelData);
        }

        [Button]
        [ShowIf("@_mode == LevelEditorMode.Create")]
        private void CreateLevel()
        {
            var levelToCreate = CreateInstance<LevelData>();

            levelToCreate.name = _levelDataEditorWrapper.LevelName;
            var tube = _levelDataEditorWrapper.Tube.GetComponent<Tube>();
            levelToCreate.SetFields(_levelDataEditorWrapper.LevelId, _levelDataEditorWrapper.BallCount,
                _levelDataEditorWrapper.BallTargetCount, tube);

            AssetDatabase.CreateAsset(levelToCreate, $"{PathHelper.LevelResourcesPath}{levelToCreate.name}.asset");
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(levelToCreate);
        }

        [Button]
        private void PopulateBalls()
        {
            if (_levelDataEditorWrapper == null || _levelDataEditorWrapper.Tube == null)
            {
                RaiseWarning("Please select a tube first.");
                return;
            }

            if (_ballPrefab == null)
            {
                _ballPrefab = AssetDatabase.LoadAssetAtPath<Ball>(PathHelper.BallPath);
            }

            Physics.simulationMode = SimulationMode.Script;

            var tubeGameObject = PrefabUtility.InstantiatePrefab(_levelDataEditorWrapper.Tube) as GameObject;
            var tube = tubeGameObject.GetComponent<Tube>();

            if (tube.Balls != null && tube.Balls.Count > 0)
            {
                foreach (var ball in tube.Balls)
                {
                    DestroyImmediate(ball.gameObject);
                }
            }

            var ballCount = _levelDataEditorWrapper.BallCount;
            EditorCoroutineUtility.StartCoroutine(BallCoroutine(tube, _ballPrefab,
                ballCount,
                () =>
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(tubeGameObject);
                    PrefabUtility.ApplyPrefabInstance(tubeGameObject, InteractionMode.UserAction);
                    EditorGUIUtility.PingObject(_levelDataEditorWrapper.Tube);
                    DestroyImmediate(tubeGameObject);
                    Physics.simulationMode = SimulationMode.FixedUpdate;
                }), this);
        }

        private IEnumerator BallCoroutine(Tube tube, Ball ball, int count,
            Action completeAction)
        {
            var addedBalls = new List<Ball>();
            var target = tube.BallParent;
            var localPos = tube.Bowl.Center.position;
            var randomOffset = Random.insideUnitSphere * 0.45f;

            for (int i = 0; i < count; i++)
            {
                EditorUtility.DisplayProgressBar("Populating Balls",
                    $"Please wait...\t {Mathf.RoundToInt(i / (float)count * 100)}%", i / (float)count);
                var addedBall = PrefabUtility.InstantiatePrefab(ball) as Ball;
                addedBall.transform.position = localPos + randomOffset;
                addedBall.transform.SetParent(target, true);

                addedBall.SetMaterial(_gameConfig.BallMaterials[Random.Range(0, _gameConfig.BallMaterials.Count)]);
                addedBall.SetPhysicMaterial(_gameConfig.BallPhysicMaterial);
                addedBalls.Add(addedBall);
                Physics.Simulate(Time.fixedDeltaTime);
                yield return new EditorWaitForSeconds(0.1f);
            }

            for (int i = 0; i < count * 2; i++)
            {
                EditorUtility.DisplayProgressBar("Simulating Ball Physics",
                    $"Please wait...\t{Mathf.RoundToInt(i / (count * 2f) * 100)}%", i / (count * 2f));
                Physics.Simulate(Time.fixedDeltaTime);
                yield return new EditorWaitForSeconds(0.2f);
            }

            tube.AssignBalls(addedBalls);
            completeAction?.Invoke();
            EditorUtility.ClearProgressBar();
        }

        private bool RaiseWarning(string message)
        {
            return EditorUtility.DisplayDialog("Warning", message, "Yes", "No");
        }

        private bool CanValidateLevel()
        {
            return true;
        }

        protected void OnEnable()
        {
            _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
            EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        protected void OnDisable()
        {
            Physics.simulationMode = SimulationMode.FixedUpdate;
            EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
        }
    }

    public enum LevelEditorMode
    {
        Create,
        Edit
    }

    [Serializable]
    public class LevelDataEditorWrapper : Object
    {
        // [HorizontalGroup("Row")]
        [VerticalGroup("Left")]
        public string LevelName;

        [VerticalGroup("Left")]
        public int LevelId;

        [VerticalGroup("Left")]
        public int BallCount;

        [VerticalGroup("Left")]
        public int BallTargetCount;

        [VerticalGroup("Right")]
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 100)]
        [HideLabel]
        [AssetSelector(Paths = "Assets/Prefabs/Game/Tubes", ExpandAllMenuItems = false,
            Filter = "t: prefab")]
        public GameObject Tube;

        public void SetFields(LevelData loadedLevelData)
        {
            LevelId = loadedLevelData.LevelId;
            BallCount = loadedLevelData.BallCount;
            BallTargetCount = loadedLevelData.BallTargetCount;
            Tube = loadedLevelData.Tube.gameObject;
            LevelName = loadedLevelData.name;
        }
    }
    
    [CustomEditor(typeof(LevelDataEditorWrapper))]
    public class LevelDataEditorWrapperEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LevelDataEditorWrapper myTarget = (LevelDataEditorWrapper)target;

            // Draw fields for LevelName, LevelId, BallCount, and BallTargetCount
            myTarget.LevelName = EditorGUILayout.TextField("Level Name", myTarget.LevelName);
            myTarget.LevelId = EditorGUILayout.IntField("Level ID", myTarget.LevelId);
            myTarget.BallCount = EditorGUILayout.IntField("Ball Count", myTarget.BallCount);
            myTarget.BallTargetCount = EditorGUILayout.IntField("Ball Target Count", myTarget.BallTargetCount);

            // Draw field for Tube with a preview
            if (GUILayout.Button("Select Tube"))
            {
                TubeSelectorWindow window = EditorWindow.GetWindow<TubeSelectorWindow>();
                window.OnTubeSelected = tube => { myTarget.Tube = tube; };
                window.Show();
            }

            if (myTarget.Tube != null)
            {
                Texture2D previewImage = AssetPreview.GetAssetPreview(myTarget.Tube);
                if (previewImage != null)
                {
                    GUILayout.Label(previewImage, GUILayout.Height(100), GUILayout.Width(100));
                }
            }
        }
    }
}