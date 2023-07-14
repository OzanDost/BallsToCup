using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DefaultNamespace;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Unity.EditorCoroutines.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Editor
{
    public class LevelEditor : OdinEditorWindow
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
        [SerializeField] private LevelDataEditorWrapper _levelDataEditorWrapper = new();
        

        private Ball _ballPrefab;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>();
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
                var addedBall = Instantiate(ball, localPos, Quaternion.identity, target);
                addedBall.transform.localPosition += randomOffset;
                addedBalls.Add(addedBall);
                Physics.Simulate(Time.fixedDeltaTime);
                yield return new EditorWaitForSeconds(0.2f);
            }

            tube.AssignBalls(addedBalls);
            completeAction?.Invoke();
        }

        private bool RaiseWarning(string message)
        {
            return EditorUtility.DisplayDialog("Warning", message, "Yes", "No");
        }

        private bool CanValidateLevel()
        {
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _ballPrefab = AssetDatabase.LoadAssetAtPath<Ball>(PathHelper.BallPath);
            Physics.simulationMode = SimulationMode.FixedUpdate;
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
        [HorizontalGroup("Row")]
        [VerticalGroup("Row/Left")]
        public string LevelName;

        [VerticalGroup("Row/Left")]
        public int LevelId;

        [VerticalGroup("Row/Left")]
        public int BallCount;

        [VerticalGroup("Row/Left")]
        public int BallTargetCount;

        [VerticalGroup("Row/Right")]
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
}