using System.IO;
using DefaultNamespace;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    public class TubeGenerator : OdinEditorWindow
    {
        [AssetSelector(Paths = PathHelper.SvgPath, ExpandAllMenuItems = false, DrawDropdownForListElements = false,
            DisableListAddButtonBehaviour = true)]
        [OnValueChanged("OnSVGChanged")]
        [SerializeField] private Object _svg;

        [Multiline(5)]
        [SerializeField] private string _svgContent;

        [SerializeField] private string _name;

        private PipeGenerator _pipeGenerator;
        private GameObject _baseTube;
        private GameObject _tubeToCreate;
        private Transform _pipeGeneratorParent;


        [MenuItem("Tools/Tube Mesh Generator")]
        public static void ShowWindow()
        {
            GetWindow<TubeGenerator>();
        }

        public void OnSVGChanged()
        {
            var svgPath = AssetDatabase.GetAssetPath(_svg).Replace("Assets/", "");
            var fullPath = Path.Combine(Application.dataPath, svgPath);
            _svgContent = File.ReadAllText(fullPath);
        }

        [Button]
        private void GenerateMesh()
        {
            if (_svgContent.IsNullOrWhitespace())
            {
                EditorUtility.DisplayDialog("Error", "SVG content is empty", "Ok");
                return;
            }

            if (_pipeGenerator == null)
            {
                LoadGenerator();
            }

            var mesh = _pipeGenerator.Generate(_svgContent);
            MeshUtility.Optimize(mesh);

            AssetDatabase.CreateAsset(mesh, $"{PathHelper.MeshSavePath}{_name}.asset");

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
            _baseTube = AssetDatabase.LoadAssetAtPath<GameObject>(PathHelper.TubeBasePath);

            var pipeGeneratorPrefab = AssetDatabase.LoadAssetAtPath<PipeGenerator>(PathHelper.PipeGeneratorPath);
            _pipeGenerator = Instantiate(pipeGeneratorPrefab, Vector3.zero, Quaternion.identity);
        }

        private void CreatePrefab(Mesh tubeMesh)
        {
            _tubeToCreate = PrefabUtility.InstantiatePrefab(_baseTube) as GameObject;
            var bowl = _tubeToCreate.GetComponent<Tube>().Bowl;
            var tube = new GameObject("Tube", typeof(MeshFilter), typeof(MeshRenderer));

            tube.GetComponent<MeshFilter>().sharedMesh = tubeMesh;
            tube.GetComponent<MeshRenderer>().sharedMaterial = _pipeGenerator.Material;
            var tubeCollider = tube.AddComponent<MeshCollider>();

            tube.transform.SetParent(bowl.transform, false);

            var tubeEdge = _pipeGenerator.SplineComputer.EvaluatePosition(0d);
            var targetTubePos = bowl.Entry.localPosition - tubeEdge;
            tube.transform.localPosition = targetTubePos;

            var totalVerticalBounds = tubeCollider.bounds.size.y + bowl.MeshCollider.bounds.size.y;
            var verticalOffset = totalVerticalBounds / 6f * Vector3.up;
            bowl.transform.localPosition -= verticalOffset;

            if (_name.Contains("Tube_"))
            {
                _name = _name.Replace("Tube_", "");
            }

            var savedTube =
                PrefabUtility.SaveAsPrefabAsset(_tubeToCreate, $"{PathHelper.TubeSavePath}Tube_{_name}.prefab");


            EditorGUIUtility.PingObject(savedTube);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // DestroyImmediate(_tubeToCreate.gameObject);
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