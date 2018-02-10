using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortableConsole
{
    [RequireComponent(typeof(PortableConsole))]
    public class PortableConsolePresenter: MonoBehaviour
    {
        private PortableConsole _portableConsole;
        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
            _portableConsole = GetComponent<PortableConsole>();
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

        //------------------------------
        // coroutines
        //------------------------------
    }
}