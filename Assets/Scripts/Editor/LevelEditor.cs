using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DefaultNamespace;
using DG.DemiEditor;
using Game;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Editor
{
    public class LevelEditor : EditorWindow
    {
        // These fields store the current state of the Level Editor
        [SerializeField] private LevelEditorMode _mode;
        [SerializeField] private LevelData _loadedLevelData;
        [SerializeField] private LevelDataEditorWrapper _levelDataEditorWrapper = new();

        // These fields are used to track changes in the Level Editor's state
        private GameConfig _gameConfig;
        private LevelEditorMode _lastMode;
        private LevelData _lastLevelData;

        // This field is a reference to the Ball prefab
        private Ball _ballPrefab;

        // This method adds a menu item for the Level Editor in the Unity Editor
        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>();
        }

        // This method is called every time the Unity Editor's GUI is updated
        private void OnGUI()
        {
            // This block of code handles the selection of the Level Editor mode
            _mode = (LevelEditorMode)GUILayout.Toolbar((int)_mode, Enum.GetNames(typeof(LevelEditorMode)));

            // This block of code handles changes in the Level Editor mode
            if (_lastMode != _mode)
            {
                OnModeChanged();
                _lastMode = _mode;
            }

            // This block of code handles changes in the loaded level data
            if (_lastLevelData != _loadedLevelData)
            {
                OnLoadedLevelChanged();
                _lastLevelData = _loadedLevelData;
                _levelDataEditorWrapper.SetFields(_loadedLevelData);
            }

            EditorGUILayout.LabelField("Level");

            // This block of code handles the display of the Tube's asset preview
            if (_levelDataEditorWrapper.Tube != null)
            {
                Texture2D previewImage = AssetPreview.GetAssetPreview(_levelDataEditorWrapper.Tube);
                if (previewImage != null)
                {
                    GUILayout.Label(previewImage);
                }
            }

            // This block of code handles the saving of the edited level

            if (_mode == LevelEditorMode.Edit)
            {
                _loadedLevelData = (LevelData)EditorGUILayout.ObjectField(_loadedLevelData, typeof(LevelData), false);

                if (_loadedLevelData != null)
                {
                    DrawLevelFields();
                }

                // This block of code handles the saving of the edited level
                if (GUILayout.Button("Save Level"))
                {
                    SaveLevel();
                }
            }
            else
            {
                DrawLevelFields();
                // This block of code handles the creation of a new level
                if (GUILayout.Button("Create Level"))
                {
                    CreateLevel();
                }
            }


            // This block of code handles the population of the level with balls
            if (_levelDataEditorWrapper == null || _levelDataEditorWrapper.Tube == null) return;
            if (GUILayout.Button("Populate Balls"))
            {
                PopulateBalls();
            }
        }

        // This method is called when the play mode state changes
        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange playMode)
        {
            // This block of code handles the transition into Play mode
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

        private void DrawLevelFields()
        {
            // This block of code handles the display and editing of the level's properties
            _levelDataEditorWrapper.LevelName =
                EditorGUILayout.TextField("Level Name", _levelDataEditorWrapper.LevelName);
            _levelDataEditorWrapper.LevelId =
                EditorGUILayout.IntField("Level ID", _levelDataEditorWrapper.LevelId);
            _levelDataEditorWrapper.BallCount =
                EditorGUILayout.IntField("Ball Count", _levelDataEditorWrapper.BallCount);
            _levelDataEditorWrapper.BallTargetCount =
                EditorGUILayout.IntField("Ball Target Count", _levelDataEditorWrapper.BallTargetCount);
            _levelDataEditorWrapper.Tube =
                (GameObject)EditorGUILayout.ObjectField("Tube", _levelDataEditorWrapper.Tube,
                    typeof(GameObject),
                    false);
        }

        // This method is called when the Level Editor mode changes
        private void OnModeChanged()
        {
            _levelDataEditorWrapper = new();
            _loadedLevelData = null;
        }

        // This method is called when the loaded level data changes
        private void OnLoadedLevelChanged()
        {
            if (_loadedLevelData == null) return;
            _levelDataEditorWrapper = new();
            _levelDataEditorWrapper.SetFields(_loadedLevelData);
        }

        // This method saves the current level data
        private void SaveLevel()
        {
            // This block of code handles the confirmation of the save operation
            if (!RaiseWarning("You are about to overwrite an existing LevelData. Are you sure?"))
            {
                return;
            }

            // This block of code handles the renaming of the level data asset
            if (!_loadedLevelData.name.Equals(_levelDataEditorWrapper.LevelName) &&
                !_levelDataEditorWrapper.LevelName.IsNullOrEmpty())
            {
                var path = AssetDatabase.GetAssetPath(_loadedLevelData);
                AssetDatabase.RenameAsset(path, _levelDataEditorWrapper.LevelName);
            }

            // This block of code handles the updating of the level data fields
            var tube = _levelDataEditorWrapper.Tube.GetComponent<Tube>();
            _loadedLevelData.SetFields(_levelDataEditorWrapper.LevelId, _levelDataEditorWrapper.BallCount,
                _levelDataEditorWrapper.BallTargetCount, tube);

            // This block of code handles the saving of the level data asset
            EditorUtility.SetDirty(_loadedLevelData);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(_loadedLevelData);
        }

        // This method creates a new level data asset
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

        // This method populates the level with balls
        private void PopulateBalls()
        {
            // This block of code handles the validation of the Tube selection
            if (_levelDataEditorWrapper == null || _levelDataEditorWrapper.Tube == null)
            {
                RaiseWarning("Please select a tube first.");
                return;
            }

            // This block of code handles the loading of the Ball prefab
            if (_ballPrefab == null)
            {
                _ballPrefab = AssetDatabase.LoadAssetAtPath<Ball>(PathHelper.BallPath);
            }

            // This block of code handles the instantiation of the Tube and the population of it with balls
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

        // This coroutine handles the physics simulation for the balls
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
    public class LevelDataEditorWrapper
    {
        public string LevelName;
        public int LevelId;
        public int BallCount;
        public int BallTargetCount;
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
}