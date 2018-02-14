#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PortableConsole
{
    public class PortableConsoleResources : ScriptableObject
    {
        public PortableConsoleStyle DefaultStyle;

        public Sprite GetLogTypeIconSprite(PortableConsoleLogType type)
        {
            switch (type)
            {
                case PortableConsoleLogType.Info:
                    return DefaultStyle.InfoSprite;
                case PortableConsoleLogType.Warning:
                    return DefaultStyle.WarningSprite;
                case PortableConsoleLogType.Error:
                default:
                    return DefaultStyle.ErrorSprite;
            }
        }

#if UNITY_EDITOR
        //todo: move it to portable console script (+ creator popup)
        [MenuItem("PortableConsole/CreateInstance")]
        public static void CreateInstance()
        {
            PortableConsoleResources resources = new PortableConsoleResources();
            AssetDatabase.CreateAsset(resources, "Assets/PortableConsole/Prefabs/PortableConsoleResources.asset");
        }
#endif
    }
}