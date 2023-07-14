using System.IO;
using DefaultNamespace;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TubeGenerator : OdinEditorWindow
    {
        [AssetSelector(Paths = SVGPath, ExpandAllMenuItems = false, DrawDropdownForListElements = false,
            DisableListAddButtonBehaviour = true)]
        [OnValueChanged("OnSVGChanged")]
        [SerializeField] private Object svg;

        [Multiline(5)]
        [SerializeField] private string svgContent;

        [SerializeField] private string Name;

        private PipeGenerator _pipeGenerator;
        private GameObject _baseTube;
        private GameObject _tubeToCreate;
        private Transform _pipeGeneratorParent;

        private const string PipeGeneratorPath = "Assets/Prefabs/Editor/PipeGenerator.prefab";
        private const string BowlPath = "Assets/Prefabs/Editor/Bowl.prefab";
        private const string TubeBasePath = "Assets/Prefabs/Game/Tubes/Tube_Base.prefab";
        private const string TubeSavePath = "Assets/Prefabs/Game/Tubes/";
        private const string MeshSavePath = "Assets/Models/TubeMeshes/";
        private const string SVGPath = "Assets/SVGs/";

        [MenuItem("Tools/Tube Mesh Generator")]
        public static void ShowWindow()
        {
            GetWindow<TubeGenerator>();
        }

        public void OnSVGChanged()
        {
            var svgPath = AssetDatabase.GetAssetPath(svg).Replace("Assets/", "");
            var fullPath = Path.Combine(Application.dataPath, svgPath);
            svgContent = File.ReadAllText(fullPath);
        }

        [Button]
        private void GenerateMesh()
        {
            if (svgContent.IsNullOrWhitespace())
            {
                EditorUtility.DisplayDialog("Error", "SVG content is empty", "Ok");
                return;
            }

            if (_pipeGenerator == null)
            {
                LoadGenerator();
            }

            var mesh = _pipeGenerator.Generate(svgContent);
            MeshUtility.Optimize(mesh);

            AssetDatabase.CreateAsset(mesh, $"{MeshSavePath}{Name}.asset");

            CreatePrefab(mesh);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ClearScene();
            LoadGenerator();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_pipeGenerator != null)
                DestroyImmediate(_pipeGenerator.gameObject);
        }

        private void LoadGenerator()
        {
            _baseTube = AssetDatabase.LoadAssetAtPath<GameObject>(TubeBasePath);

            var pipeGeneratorPrefab = AssetDatabase.LoadAssetAtPath<PipeGenerator>(PipeGeneratorPath);
            _pipeGenerator = Instantiate(pipeGeneratorPrefab, Vector3.zero, Quaternion.identity);
        }

        private void CreatePrefab(Mesh tubeMesh)
        {
            _tubeToCreate = PrefabUtility.InstantiatePrefab(_baseTube) as GameObject;
            var bowl = _tubeToCreate.GetComponent<Tube>().Bowl;
            var tube = new GameObject("Tube", typeof(MeshFilter), typeof(MeshRenderer));

            tube.GetComponent<MeshFilter>().sharedMesh = tubeMesh;
            tube.GetComponent<MeshRenderer>().sharedMaterial = _pipeGenerator.Material;
            tube.AddComponent<MeshCollider>();

            tube.transform.SetParent(bowl.transform, false);

            var tubeEdge = tube.transform.InverseTransformPoint(_pipeGenerator.SplineComputer.EvaluatePosition(0d));
            var targetTubePos = bowl.Entry.position - tubeEdge;
            tube.transform.localPosition = targetTubePos;

            if (Name.Contains("Tube_"))
            {
                Name = Name.Replace("Tube_", "");
            }

            var savedTube = PrefabUtility.SaveAsPrefabAsset(_tubeToCreate, $"{TubeSavePath}Tube_{Name}.prefab");


            EditorGUIUtility.PingObject(savedTube);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            DestroyImmediate(_tubeToCreate.gameObject);
        }

        private void ClearScene()
        {
            var generatorsInScene = FindObjectsOfType<PipeGenerator>();

            foreach (var generator in generatorsInScene)
            {
                DestroyImmediate(generator.gameObject);
            }
        }
    }
}