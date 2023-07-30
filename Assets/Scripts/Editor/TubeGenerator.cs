using System.IO;
using DG.DemiEditor;
using Game;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    public class TubeGenerator : EditorWindow
    {
        [SerializeField] private Object _svg;

        [TextArea]
        [SerializeField] private string _svgContent;

        [SerializeField] private string _name;

        private PipeGenerator _pipeGenerator;
        private GameObject _baseTube;
        private GameObject _tubeToCreate;
        private Transform _pipeGeneratorParent;

        private Object _lastSvg;


        [MenuItem("Tools/Tube Mesh Generator")]
        public static void ShowWindow()
        {
            GetWindow<TubeGenerator>();
        }

        private void OnSVGChanged()
        {
            var svgPath = AssetDatabase.GetAssetPath(_svg).Replace("Assets/", "");
            var fullPath = Path.Combine(Application.dataPath, svgPath);
            _svgContent = File.ReadAllText(fullPath);
        }

        private void OnGUI()
        {
            _svg = EditorGUILayout.ObjectField("SVG", _svg, typeof(Object), false);
            if (_svg != null && _svg != _lastSvg)
            {
                OnSVGChanged();
                _lastSvg = _svg;
            }
        
            _svgContent = EditorGUILayout.TextField("SVG Content", _svgContent);
            _name = EditorGUILayout.TextField("Name", _name);

            if (GUILayout.Button("Generate Mesh"))
            {
                GenerateMesh();
            }
        }
        private void GenerateMesh()
        {
            if (_svgContent.IsNullOrEmpty())
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

        private void OnEnable()
        {

            ClearScene();
            LoadGenerator();
        }

        private void OnDisable()
        {
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

            var ballReleaserAsset = AssetDatabase.LoadAssetAtPath<BallReleaser>(PathHelper.BallReleaserPath);
            var ballReleaser = PrefabUtility.InstantiatePrefab(ballReleaserAsset) as BallReleaser;
            ballReleaser.transform.SetParent(tube.transform, false);

            tube.GetComponent<MeshFilter>().sharedMesh = tubeMesh;
            tube.GetComponent<MeshRenderer>().sharedMaterial = _pipeGenerator.Material;
            var tubeCollider = tube.AddComponent<MeshCollider>();

            tube.transform.SetParent(bowl.transform, false);

            var tubeStart = _pipeGenerator.SplineComputer.EvaluatePosition(0d);

            var tubeEnd = _pipeGenerator.SplineComputer.EvaluatePosition(1d);
            tubeEnd.x -= tube.transform.localPosition.x;

            var targetTubePos = bowl.Entry.localPosition - tubeStart;
            tube.transform.localPosition = targetTubePos;
            ballReleaser.transform.localPosition = tubeEnd;

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