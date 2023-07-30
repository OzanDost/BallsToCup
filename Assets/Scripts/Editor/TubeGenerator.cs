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

        // This method is used to create a prefab from the generated tube mesh.
        private void CreatePrefab(Mesh tubeMesh)
        {
            // Instantiate a copy of the base tube object.
            _tubeToCreate = PrefabUtility.InstantiatePrefab(_baseTube) as GameObject;

            // Get the Bowl component from the tube object.
            var bowl = _tubeToCreate.GetComponent<Tube>().Bowl;

            // Create a new GameObject named "Tube" with MeshFilter and MeshRenderer components.
            var tube = new GameObject("Tube", typeof(MeshFilter), typeof(MeshRenderer));

            // Load the BallReleaser asset and instantiate a copy of it.
            var ballReleaserAsset = AssetDatabase.LoadAssetAtPath<BallReleaser>(PathHelper.BallReleaserPath);
            var ballReleaser = PrefabUtility.InstantiatePrefab(ballReleaserAsset) as BallReleaser;

            // Set the parent of the ball releaser to the tube.
            ballReleaser.transform.SetParent(tube.transform, false);

            // Set the mesh and material of the tube.
            tube.GetComponent<MeshFilter>().sharedMesh = tubeMesh;
            tube.GetComponent<MeshRenderer>().sharedMaterial = _pipeGenerator.Material;

            // Add a MeshCollider to the tube.
            var tubeCollider = tube.AddComponent<MeshCollider>();

            // Set the parent of the tube to the bowl.
            tube.transform.SetParent(bowl.transform, false);

            // Calculate the start and end positions of the tube.
            var tubeStart = _pipeGenerator.SplineComputer.EvaluatePosition(0d);
            var tubeEnd = _pipeGenerator.SplineComputer.EvaluatePosition(1d);
            tubeEnd.x -= tube.transform.localPosition.x;

            // Adjust the position of the tube and the ball releaser.
            var targetTubePos = bowl.Entry.localPosition - tubeStart;
            tube.transform.localPosition = targetTubePos;
            ballReleaser.transform.localPosition = tubeEnd;

            // Adjust the position of the bowl.
            var totalVerticalBounds = tubeCollider.bounds.size.y + bowl.MeshCollider.bounds.size.y;
            var verticalOffset = totalVerticalBounds / 6f * Vector3.up;
            bowl.transform.localPosition -= verticalOffset;

            // Remove the prefix "Tube_" from the name if it exists.
            if (_name.Contains("Tube_"))
            {
                _name = _name.Replace("Tube_", "");
            }

            // Save the tube object as a prefab and get the saved prefab.
            var savedTube =
                PrefabUtility.SaveAsPrefabAsset(_tubeToCreate, $"{PathHelper.TubeSavePath}Tube_{_name}.prefab");

            // Highlight the saved prefab in the Project window.
            EditorGUIUtility.PingObject(savedTube);

            // Save all unsaved asset changes.
            AssetDatabase.SaveAssets();

            // Refresh the AssetDatabase to reflect these changes.
            AssetDatabase.Refresh();

            // Destroy the tube object in the scene.
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