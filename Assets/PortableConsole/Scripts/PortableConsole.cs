using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PortableConsole
{
    [System.Flags]
    public enum PortableConsoleLogType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 4,
    }

    public class PortableConsole : MonoBehaviour
    {
        public PortableConsoleResources Resources;

        public GameObject LogTemplate;

        public Color FirstColor;
        public Color SecondColor;

        public RectTransform Content;

        private GameObject _consoleContent;

        private uint _infoCounter;
        private uint _warningCounter;
        private uint _errorCounter;

        private Text InfoCounter;
        private Text WarningCounter;
        private Text ErrorCounter;

        private List<PortableConsoleLog> _logs = new List<PortableConsoleLog>();
        private PortableConsoleLogType _logFilter = PortableConsoleLogType.Info | PortableConsoleLogType.Warning | PortableConsoleLogType.Error;
        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
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
            foreach (Transform child in Content.transform)
            {
                Destroy(child.gameObject);
            }

            Content.sizeDelta = Vector2.zero;

            _logs.Clear();
            _infoCounter = _warningCounter = _errorCounter = 0;

            UpdateLogCount();
        }

        public void OnClickInfoButton()
        {
            ChangeFilter(PortableConsoleLogType.Info);
        }

        public void OnClickWarningButton()
        {
            ChangeFilter(PortableConsoleLogType.Warning);
        }

        public void OnClickErrorButton()
        {
            ChangeFilter(PortableConsoleLogType.Error);
        }

        public void OnClickCloseButton()
        {
            CloseConsoleContent();
        }

        //------------------------------
        // private methods
        //------------------------------
        private void ChangeFilter(PortableConsoleLogType type)
        {
            if ((_logFilter & type) == type)
            {
                _logFilter = _logFilter & ~type;
            }
            else
            {
                _logFilter = (_logFilter | type);
            }

            RedrawConsoleLogs();
        }

        private void Setup()
        {
            if (Resources == null)
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
            _logs.Add(new PortableConsoleLog(type, condition, stackTrace));

            var currentAdded = _logs[_logs.Count - 1];

            uint currentCount = 0;
            bool draw = false;

            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    {
                        if ((_logFilter & PortableConsoleLogType.Error) == PortableConsoleLogType.Error)
                        {
                            draw = true;
                            currentCount += _errorCounter;

                            _errorCounter++;
                        }
                    }
                    break;
                case LogType.Log:
                    {
                        if ((_logFilter & PortableConsoleLogType.Info) == PortableConsoleLogType.Info)
                        {
                            draw = true;
                            currentCount += _warningCounter;

                            _warningCounter++;
                        }
                    }
                    break;
                case LogType.Warning:
                    {
                        if ((_logFilter & PortableConsoleLogType.Warning) == PortableConsoleLogType.Warning)
                        {
                            draw = true;
                            currentCount += _infoCounter;

                            _infoCounter++;
                        }
                    }
                    break;
            }


            if (draw)
            {
                DrawConsoleLog(currentAdded, currentCount);
            }

            UpdateLogCount();
        }

        private void RedrawConsoleLogs()
        {
            foreach (Transform t in Content.transform)
            {
                Destroy(t.gameObject);
            }
            Content.sizeDelta = Vector2.zero;

            uint i = 0;
            foreach (var l in _logs)
            {
                switch (l.LogType)
                {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        {
                            if ((_logFilter & PortableConsoleLogType.Error) != PortableConsoleLogType.Error)
                            {
                                continue;
                            }
                        }
                        break;
                    case LogType.Log:
                        {
                            if ((_logFilter & PortableConsoleLogType.Info) != PortableConsoleLogType.Info)
                            {
                                continue;
                            }
                        }
                        break;
                    case LogType.Warning:
                        {
                            if ((_logFilter & PortableConsoleLogType.Warning) != PortableConsoleLogType.Warning)
                            {
                                continue;
                            }
                        }
                        break;
                }

                DrawConsoleLog(l, i);

                i++;
            }
        }

        private void DrawConsoleLog(PortableConsoleLog log, uint counter)
        {
            //create instance
            var g = Instantiate(LogTemplate, Content.transform).GetComponent<RectTransform>();

            //set background color
            var bg = g.GetComponent<Image>();
            bg.color = ((counter & 1) == 0) ? FirstColor : SecondColor;

            //set correspond image
            var icon = g.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = Resources.GetLogTypeIconSprite(log.LogType);

            //update text content
            var logContent = g.transform.Find("Content").GetComponent<Text>();
            var logCaster = g.transform.Find("Caster").GetComponent<Text>();

            logContent.text = log.Name;
            logCaster.text = log.Caster;

            //manage item's position and size
            var size = new Vector2(0, 100);
            var offset = (size / 2f) + size * counter;

            g.sizeDelta = size;
            g.anchoredPosition -= offset;

            //update scroll rect's content size
            Content.sizeDelta += size;
        }

        private void UpdateLogCount()
        {
            InfoCounter.text = GetCounterText(_infoCounter);
            WarningCounter.text = GetCounterText(_warningCounter);
            ErrorCounter.text = GetCounterText(_errorCounter);
        }

        private string GetCounterText(uint value)
        {
            if (value < 100)
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