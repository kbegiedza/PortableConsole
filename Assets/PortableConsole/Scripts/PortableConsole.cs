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

        private PortableConsoleLogType _logType = PortableConsoleLogType.Info | PortableConsoleLogType.Warning | PortableConsoleLogType.Error;
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
            foreach(Transform child in Content.transform)
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
            if ((_logType & PortableConsoleLogType.Info) == PortableConsoleLogType.Info)
            {
                _logType = _logType & ~PortableConsoleLogType.Info;
            }
            else
            {
                _logType = (_logType | PortableConsoleLogType.Info);
            }

            DrawConsoleLogs();
        }

        public void OnClickWarningButton()
        {
            if((_logType & PortableConsoleLogType.Warning) == PortableConsoleLogType.Warning)
            {
                _logType = _logType & ~PortableConsoleLogType.Warning;
            }
            else
            {
                _logType = (_logType | PortableConsoleLogType.Warning);
            }

            DrawConsoleLogs();
        }

        public void OnClickErrorButton()
        {
            if ((_logType & PortableConsoleLogType.Error) == PortableConsoleLogType.Error)
            {
                _logType = _logType & ~PortableConsoleLogType.Error;
            }
            else
            {
                _logType = (_logType | PortableConsoleLogType.Error);
            }

            DrawConsoleLogs();
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
            uint _logCount = 0;
            bool draw = false;

            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    {
                        if((_logType & PortableConsoleLogType.Error) == PortableConsoleLogType.Error)
                        {
                            draw = true;
                        }
                    } 
                    break;
                case LogType.Log:
                    {
                        if ((_logType & PortableConsoleLogType.Info) == PortableConsoleLogType.Info)
                        {
                            draw = true;
                        }
                    }
                    break;
                case LogType.Warning:
                    {
                        if ((_logType & PortableConsoleLogType.Warning) == PortableConsoleLogType.Warning)
                        {
                            draw = true;
                        }
                    }
                    break;
            }

            if ((_logType & PortableConsoleLogType.Error) == PortableConsoleLogType.Error)
            {
                _logCount += _errorCounter;
            }

            if ((_logType & PortableConsoleLogType.Warning) == PortableConsoleLogType.Warning)
            {
                _logCount += _warningCounter;
            }

            if ((_logType & PortableConsoleLogType.Info) == PortableConsoleLogType.Info)
            {
                _logCount += _infoCounter;
            }


            if(draw)
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
            }

            _logs.Add(new PortableConsoleLog(type, condition, stackTrace));

            switch (type)
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

        private void DrawConsoleLogs()
        {
            foreach(Transform t in Content.transform)
            {
                Destroy(t.gameObject);
            }
            Content.sizeDelta = Vector2.zero;

            int i = 0;
            foreach (var l in _logs)
            {

                switch (l.LogType)
                {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        {
                            if ((_logType & PortableConsoleLogType.Error) != PortableConsoleLogType.Error)
                            {
                                continue;
                            }
                        }
                        break;
                    case LogType.Log:
                        {
                            if ((_logType & PortableConsoleLogType.Info) != PortableConsoleLogType.Info)
                            {
                                continue;
                            }
                        }
                        break;
                    case LogType.Warning:
                        {
                            if ((_logType & PortableConsoleLogType.Warning) != PortableConsoleLogType.Warning)
                            {
                                continue;
                            }
                        }
                        break;
                }

                //create instance
                var g = Instantiate(LogTemplate, Content.transform).GetComponent<RectTransform>();

                //set background color
                var bg = g.GetComponent<Image>();
                bg.color = ((i & 1) == 0) ? FirstColor : SecondColor;

                //set correspond image
                var icon = g.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = Resources.GetLogTypeIconSprite(l.LogType);

                //update text content
                var logContent = g.transform.Find("Content").GetComponent<Text>();
                var logCaster = g.transform.Find("Caster").GetComponent<Text>();

                logContent.text = l.Name;
                logCaster.text = l.Caster;

                //manage item's position and size
                var size = new Vector2(0, 100);
                var offset = (size / 2f) + size * i;

                g.sizeDelta = size;
                g.anchoredPosition -= offset;

                //update scroll rect's content size
                Content.sizeDelta += size;
                i++;
            }
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