using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Rigidbody))]
    public class RigidbodyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Rigidbody rigidbody = (Rigidbody)target;

            rigidbody.mass = EditorGUILayout.FloatField("Mass", rigidbody.mass);
            rigidbody.drag = EditorGUILayout.FloatField("Drag", rigidbody.drag);
            rigidbody.angularDrag = EditorGUILayout.FloatField("Angular Drag", rigidbody.angularDrag);
            rigidbody.useGravity = EditorGUILayout.Toggle("Use Gravity", rigidbody.useGravity);
            rigidbody.isKinematic = EditorGUILayout.Toggle("Is Kinematic", rigidbody.isKinematic);
            rigidbody.interpolation = (RigidbodyInterpolation)EditorGUILayout.EnumPopup("Interpolation", rigidbody.interpolation);
            rigidbody.collisionDetectionMode = (CollisionDetectionMode)EditorGUILayout.EnumPopup("Collision Detection", rigidbody.collisionDetectionMode);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(rigidbody);
            }
        }
    }

}