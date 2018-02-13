using UnityEngine;

namespace PortableConsole
{
    [System.Flags]
    public enum PortableConsoleLogType
    {
        Info = 1,
        Warning = 2,
        Error = 4,
    }

    public class PortableConsoleLog
    {
        public PortableConsoleLogType LogType { get; private set; }

        public string Name { get; set; }
        public string Caster { get; set; }

        public string Details { get; set; }
        //------------------------------
        // public methods
        //------------------------------
        public PortableConsoleLog(LogType type, string name, string details)
        {
            LogType = GetFromLogType(type);
            Name = name;
            Details = details;
            Caster = details.Trim(' ');
        }

        //------------------------------
        // private methods
        //------------------------------
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
        //------------------------------
        // coroutines
        //------------------------------
    }
}