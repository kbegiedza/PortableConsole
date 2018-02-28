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
        private SerializedProperty _useDefaultButton;

        private bool _resourceToggle = false;
        private bool _advancedToggle = false;
        //------------------------------
        // Unity methods
        //------------------------------

        [MenuItem("PortableConsole/CreateInstance")]
        public static void CreateInstance()
        {
            PortableConsoleResources resources = new PortableConsoleResources();
            AssetDatabase.CreateAsset(resources, "Assets/PortableConsole/Prefabs/PortableConsoleResources.asset");
        }

        private void OnEnable()
        {
            _resources = serializedObject.FindProperty("_resources");
            _logTemplate = serializedObject.FindProperty("_logTemplate");
            _toggleButton = serializedObject.FindProperty("_toggleButton");
            _useDefaultButton = serializedObject.FindProperty("_useDefaultButton");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            var script = (PortableConsole)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_useDefaultButton);
            var defaultChanged = EditorGUI.EndChangeCheck();
            if (_useDefaultButton.boolValue)
            {
                if(defaultChanged)
                {
                    _toggleButton.objectReferenceValue = script.gameObject.transform.Find("Canvas").Find("DefaultOpenButton").gameObject;
                }

                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUILayout.PropertyField(_toggleButton);
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (defaultChanged)
                {
                    _toggleButton.objectReferenceValue = null;
                }

                EditorGUILayout.PropertyField(_toggleButton);
            }

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