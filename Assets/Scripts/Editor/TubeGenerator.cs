using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TubeGenerator : OdinEditorWindow
    {
        [AssetSelector(Paths = "Assets/SVGs")]
        [OnValueChanged("OnSVGChanged")]
        [SerializeField] private Object svg;

        [Multiline(5)]
        [SerializeField] private string svgContent;

        [SerializeField] private string ID;

        private PipeGenerator _pipeGenerator;

        private const string PipeGeneratorPath = "Assets/Editor/LevelEditorResources/Prefabs/PipeGenerator.prefab";
        private const string MeshSavePath = "Assets/Models/Meshes/";

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

            AssetDatabase.CreateAsset(mesh, $"{MeshSavePath}{ID}.asset");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            LoadGenerator();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DestroyImmediate(_pipeGenerator.gameObject);
        }

        private void LoadGenerator()
        {
            var pipeGeneratorPrefab = AssetDatabase.LoadAssetAtPath<PipeGenerator>(PipeGeneratorPath);
            _pipeGenerator = Instantiate(pipeGeneratorPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}