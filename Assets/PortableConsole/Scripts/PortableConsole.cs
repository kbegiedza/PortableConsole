using UnityEngine;

namespace PortableConsole
{
    public class PortableConsole : MonoBehaviour
    {
        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
            Application.logMessageReceived += LogMessageReceived;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        //------------------------------
        // public methods
        //------------------------------

        //------------------------------
        // private methods
        //------------------------------
        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Debug.LogFormat("Logged type: {0}, condition: {1}", type.ToString(), condition);
        }

        //------------------------------
        // coroutines
        //------------------------------
    }
}