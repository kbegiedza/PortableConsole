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
    }
}