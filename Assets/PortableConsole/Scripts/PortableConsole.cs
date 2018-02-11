using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PortableConsole
{
    public class PortableConsole : MonoBehaviour
    {
        public static bool Enabled { get; set; }

        public PortableConsoleResources Resources;

        public GameObject LogTemplate;

        public Color FirstColor;
        public Color SecondColor;

        public RectTransform Content;

        private int _logCount = 0;

        private GameObject _consoleContent;

        private uint _infoCounter;
        private uint _warningCounter;
        private uint _errorCounter;

        private Text InfoCounter;
        private Text WarningCounter;
        private Text ErrorCounter;

        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
            Enabled = true;

            Setup();

            //attach our logger to Unity's event
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
        public void OnClickClearButton()
        {
            foreach(Transform child in Content.transform)
            {
                Destroy(child.gameObject);
            }

            Content.sizeDelta = Vector2.zero;

            _logCount = 0;
            _infoCounter = _warningCounter = _errorCounter = 0;

            UpdateLogCount();
        }

        public void OnClickInfoButton()
        {
            Debug.Log("Info");
        }

        public void OnClickWarningButton()
        {
            Debug.LogWarning("Warning");
        }

        public void OnClickErrorButton()
        {
            Debug.LogError("Error");
        }

        public void OnClickCloseButton()
        {
            CloseConsoleContent();
        }

        //------------------------------
        // private methods
        //------------------------------
        private void Setup()
        {
            if(Resources == null)
            {
                throw new System.NullReferenceException("PortableConsoleResources is null!");
            }

            _consoleContent = transform.Find("PortableConsoleContent").gameObject;

            var firstTopBar = _consoleContent.transform.Find("TopBar").Find("FirstTopBar");
            InfoCounter = firstTopBar.Find("ToggleInfoButton").Find("Counter").GetComponent<Text>();
            WarningCounter = firstTopBar.Find("ToggleWarningButton").Find("Counter").GetComponent<Text>();
            ErrorCounter = firstTopBar.Find("ToggleErrorButton").Find("Counter").GetComponent<Text>();

            UpdateLogCount();
        }

        private void OpenConsoleContent()
        {
            _consoleContent.SetActive(true);
        }

        private void CloseConsoleContent()
        {
            _consoleContent.SetActive(false);
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            //create instance
            var g = Instantiate(LogTemplate, Content.transform).GetComponent<RectTransform>();

            //set background color
            var bg = g.GetComponent<Image>();
            bg.color = ((_logCount & 1) == 0) ? FirstColor : SecondColor;

            //set correspond image
            var icon = g.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = Resources.GetLogTypeIconSprite(type);

            //update text content
            var logContent = g.transform.Find("Content").GetComponent<Text>();
            var logCaster = g.transform.Find("Caster").GetComponent<Text>();

            logContent.text = condition;
            logCaster.text = stackTrace.Trim(' ');

            //manage item's position and size
            var size = new Vector2(0, 100);
            var offset = (size/2f) + size * _logCount;

            g.sizeDelta = size;
            g.anchoredPosition -= offset;

            //update scroll rect's content size
            Content.sizeDelta += size;

            _logCount++;

            switch(type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    _errorCounter++;
                    break;
                case LogType.Log:
                    _infoCounter++;
                    break;
                case LogType.Warning:
                    _warningCounter++;
                    break;
            }

            UpdateLogCount();
        }

        private void UpdateLogCount()
        {
            InfoCounter.text = GetCounterText(_infoCounter);
            WarningCounter.text = GetCounterText(_warningCounter);
            ErrorCounter.text = GetCounterText(_errorCounter);
        }

        private string GetCounterText(uint value)
        {
            if(value < 100)
            {
                return value.ToString();
            }
            else
            {
                return "99+";
            }
        }
        //------------------------------
        // coroutines
        //------------------------------
    }
}