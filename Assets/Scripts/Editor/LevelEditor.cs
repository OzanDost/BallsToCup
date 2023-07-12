using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Editor
{
    public class LevelEditor : OdinEditorWindow
    {
        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}