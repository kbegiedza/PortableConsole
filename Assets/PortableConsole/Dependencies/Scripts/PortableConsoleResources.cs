using UnityEngine;

namespace PortableConsole
{
    /// <summary>
    /// <see cref="PortableConsole"/>'s resource handler
    /// </summary>
    public class PortableConsoleResources : ScriptableObject
    {
        public PortableConsoleStyle DefaultStyle;

        /// <summary>
        /// Returns icon's <see cref="Sprite"/> depended on <see cref="PortableConsoleLogType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Sprite GetIconSpriteFromLogType(PortableConsoleLogType type)
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

        /// <summary>
        /// Returns <see cref="Color"/> depended on <see cref="PortableConsoleLogType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Color GetColorFromLogType(PortableConsoleLogType type)
        {
            switch (type)
            {
                case PortableConsoleLogType.Info:
                    return DefaultStyle.LogColor;
                case PortableConsoleLogType.Warning:
                    return DefaultStyle.WarningColor;
                case PortableConsoleLogType.Error:
                default:
                    return DefaultStyle.ErrorColor;
            }
        }
    }
}