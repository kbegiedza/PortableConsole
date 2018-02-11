#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PortableConsole
{
    public class PortableConsoleResources : ScriptableObject
    {
        public Sprite InfoSprite;
        public Sprite WarningSprite;
        public Sprite ErrorSprite;

        public Sprite GetLogTypeIconSprite(LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    return ErrorSprite;
                case LogType.Log:
                    return InfoSprite;
                case LogType.Warning:
                    return WarningSprite;
                default:
                    return ErrorSprite;
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