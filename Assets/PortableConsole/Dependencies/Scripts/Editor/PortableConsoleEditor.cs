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

        private PortableConsole _script;
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

            _script = (PortableConsole)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            DrawToggleButtonSection();
            EditorGUILayout.Space();
            DrawAdvancedOptions();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws toggle button selection section
        /// </summary>
        private void DrawToggleButtonSection()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_useDefaultButton);
            var defaultChanged = EditorGUI.EndChangeCheck();
            if (_useDefaultButton.boolValue)
            {
                if (defaultChanged)
                {
                    var gameObject = _script.gameObject.transform.Find("Canvas").Find("DefaultOpenButton").gameObject;
                    gameObject.SetActive(true);
                    _toggleButton.objectReferenceValue = gameObject;
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
                    var gameObject = _script.gameObject.transform.Find("Canvas").Find("DefaultOpenButton").gameObject;
                    gameObject.SetActive(false);
                    _toggleButton.objectReferenceValue = null;
                }

                EditorGUILayout.PropertyField(_toggleButton);
            }
        }

        /// <summary>
        /// Draws advanced options section
        /// </summary>
        private void DrawAdvancedOptions()
        {
            _advancedToggle = EditorGUILayout.Foldout(_advancedToggle, "Advanced options:");
            if (_advancedToggle)
            {
                if (GUILayout.Button(_resourceToggle ? "Lock" : "Unlock"))
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
        }
    }
}