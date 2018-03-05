using UnityEngine;

namespace PortableConsole
{
    public class PortableConsoleTestScript : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("F1");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.LogWarning("F2");
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.LogError("F3");
            }

            if (Input.GetKey(KeyCode.F4))
            {
                Debug.LogError("holding F4");
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                throw new System.NotImplementedException("Some message");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                GameObject obj = null;
                obj.name = "nullptr";
            }
        }
    }
}