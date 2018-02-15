using UnityEngine;
using UnityEditor;

namespace PortableConsole
{
    [CustomEditor(typeof(PortableConsole))]
    [CanEditMultipleObjects]
    public class PortableConsoleEditor : Editor
    {
        private SerializedProperty _resources;
        private SerializedProperty _logTemplate;
        private SerializedProperty _toggleButton;
        //------------------------------
        // Unity methods
        //------------------------------

        private void OnEnable()
        {
            _resources = serializedObject.FindProperty("_resources");
            _logTemplate = serializedObject.FindProperty("_logTemplate");
            _toggleButton = serializedObject.FindProperty("_toggleButton");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_toggleButton, true);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_resources,true);

            EditorGUILayout.PropertyField(_logTemplate, true);
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
        //------------------------------
        // public methods
        //------------------------------

        //------------------------------
        // private methods
        //------------------------------

        //------------------------------
        // coroutines
        //------------------------------
    }
}