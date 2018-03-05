using UnityEngine;

namespace PortableConsole
{
    /// <summary>
    /// Flag enum for <see cref="PortableConsole"/>'s logs
    /// </summary>
    [System.Flags]
    [System.Serializable]
    public enum PortableConsoleLogType
    {
        Info = 1,
        Warning = 2,
        Error = 4,
    }

    /// <summary>
    /// <see cref="PortableConsole"/>'s logs
    /// </summary>
    public class PortableConsoleLog
    {
        public PortableConsoleLogType LogType { get; private set; }
        public string Name { get; set; }
        public string Caster { get; set; }
        public string Details { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PortableConsoleLog"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="details"></param>
        public PortableConsoleLog(LogType type, string name, string details)
        {
            LogType = GetFromLogType(type);
            Name = name;
            Details = details;
            Caster = details.Trim(System.Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Converts Unity's <see cref="UnityEngine.LogType"/> to <see cref="PortableConsoleLogType"/>.
        /// If can't convert returns <see cref="PortableConsoleLogType.Error"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private PortableConsoleLogType GetFromLogType(LogType type)
        {
            switch (type)
            {
                case UnityEngine.LogType.Log:
                    return PortableConsoleLogType.Info;
                case UnityEngine.LogType.Warning:
                    return PortableConsoleLogType.Warning;
                case UnityEngine.LogType.Assert:
                case UnityEngine.LogType.Error:
                case UnityEngine.LogType.Exception:
                default:
                    return PortableConsoleLogType.Error;
            }
        }
    }
}