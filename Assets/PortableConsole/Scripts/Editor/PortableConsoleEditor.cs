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

        private bool _resourceToggle = false;
        private bool _advancedToggle = false;
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

            EditorGUILayout.Space();

            _advancedToggle = EditorGUILayout.Foldout(_advancedToggle, "Advanced options:");
            if (_advancedToggle)
            {
                if(GUILayout.Button(_resourceToggle ? "Lock" : "Unlock"))
                {
                    _resourceToggle = !_resourceToggle;
                }

                EditorGUI.BeginDisabledGroup(!_resourceToggle);
                {
                    EditorGUILayout.PropertyField(_resources, true);
                    EditorGUILayout.PropertyField(_logTemplate, true);
                }
                EditorGUI.EndDisabledGroup();
            }


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