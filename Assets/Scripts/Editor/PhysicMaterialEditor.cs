using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PhysicMaterial))]
    public class PhysicMaterialEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PhysicMaterial physicMaterial = (PhysicMaterial)target;

            physicMaterial.dynamicFriction = EditorGUILayout.FloatField("Dynamic Friction", physicMaterial.dynamicFriction);
            physicMaterial.staticFriction = EditorGUILayout.FloatField("Static Friction", physicMaterial.staticFriction);
            physicMaterial.bounciness = EditorGUILayout.FloatField("Bounciness", physicMaterial.bounciness);
            physicMaterial.frictionCombine = (PhysicMaterialCombine)EditorGUILayout.EnumPopup("Friction Combine", physicMaterial.frictionCombine);
            physicMaterial.bounceCombine = (PhysicMaterialCombine)EditorGUILayout.EnumPopup("Bounce Combine", physicMaterial.bounceCombine);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(physicMaterial);
            }
        }
    }
}