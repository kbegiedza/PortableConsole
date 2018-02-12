using UnityEngine;

namespace PortableConsole
{
    public class PortableConsoleLog
    {
        public LogType LogType { get; private set; }

        public string Name { get; set; }
        public string Caster { get; set; }

        public string Details { get; set; }
        //------------------------------
        // public methods
        //------------------------------
        public PortableConsoleLog(LogType type, string name, string details)
        {
            LogType = type;
            Name = name;
            Details = details;
            Caster = details.Trim(' ');
        }

        //------------------------------
        // private methods
        //------------------------------

        //------------------------------
        // coroutines
        //------------------------------
    }
}