using Data;
using DefaultNamespace;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    public class GameConfigEditor : OdinEditorWindow
    {
        private GameConfig _gameConfig;
        private PropertyTree _propertyTree;

        [MenuItem("Tools/Game Config Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameConfigEditor>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
            _propertyTree = PropertyTree.Create(_gameConfig);
        }

        protected override void DrawEditors()
        {
            base.DrawEditors();
            if (_gameConfig == null)
            {
                _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
            }

            if (_propertyTree != null)
            {
                _propertyTree = PropertyTree.Create(_gameConfig);
            }

            _propertyTree?.Draw();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _propertyTree?.Dispose();
        }
    }
}