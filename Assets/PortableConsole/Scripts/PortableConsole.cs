using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PortableConsole
{
    public class PortableConsole : MonoBehaviour
    {
        private static string _defaultEventSystemName = "DefaultEventSystem";

        public PortableConsoleResources Resources;
        public GameObject LogTemplate;
        public Button ToggleButton;
        public RectTransform Content;

        private GameObject _consoleContent;

        private Text InfoCounter;
        private Text WarningCounter;
        private Text ErrorCounter;

        private List<PortableConsoleLog> _logs = new List<PortableConsoleLog>();
        private PortableConsoleLogType _logFilter = PortableConsoleLogType.Info | PortableConsoleLogType.Warning | PortableConsoleLogType.Error;

        private Dictionary<PortableConsoleLogType, uint> _logCounters = new Dictionary<PortableConsoleLogType, uint>
        {
            { PortableConsoleLogType.Info, 0},
            { PortableConsoleLogType.Warning, 0},
            { PortableConsoleLogType.Error, 0},
        };

        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
            Setup();

            ToggleButton.onClick.AddListener(ToggleContent);
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
            foreach(var key in _logCounters.Keys.ToList())
            {
                _logCounters[key] = 0;
            }

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
            HideConsoleContent();
        }

        //------------------------------
        // private methods
        //------------------------------
        private void ToggleContent()
        {
            if (_consoleContent.activeInHierarchy)
            {
                HideConsoleContent();
            }
            else
            {
                ShowConsoleContent();
            }
        }

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
            //check for EventSystem and create if required
            var eventSystem = FindObjectOfType<EventSystem>();
            if(eventSystem == null)
            {
                GameObject obj = new GameObject(_defaultEventSystemName);
                obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>();
            }

            if (Resources == null)
            {
                throw new System.NullReferenceException("PortableConsoleResources is null!");
            }

            _consoleContent = transform.Find("Canvas").Find("Console").gameObject;

            var firstTopBar = _consoleContent.transform.Find("TopBar").Find("FirstTopBar");
            InfoCounter = firstTopBar.Find("ToggleInfoButton").Find("Counter").GetComponent<Text>();
            WarningCounter = firstTopBar.Find("ToggleWarningButton").Find("Counter").GetComponent<Text>();
            ErrorCounter = firstTopBar.Find("ToggleErrorButton").Find("Counter").GetComponent<Text>();

            //attach our logger to Unity's event
            Application.logMessageReceived += LogMessageReceived;

            UpdateLogCount();
        }

        private void ShowConsoleContent()
        {
            _consoleContent.SetActive(true);

            ToggleButton.gameObject.SetActive(false);
        }

        private void HideConsoleContent()
        {
            _consoleContent.SetActive(false);

            ToggleButton.gameObject.SetActive(true);
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            _logs.Add(new PortableConsoleLog(type, condition, stackTrace));

            var currentAdded = _logs[_logs.Count - 1];

            uint currentCount = 0;
            bool draw = false;

            if ((_logFilter & PortableConsoleLogType.Error) == PortableConsoleLogType.Error)
            {
                currentCount += _logCounters[PortableConsoleLogType.Error];
            }

            if ((_logFilter & PortableConsoleLogType.Info) == PortableConsoleLogType.Info)
            {
                currentCount += _logCounters[PortableConsoleLogType.Info];
            }
            if ((_logFilter & PortableConsoleLogType.Warning) == PortableConsoleLogType.Warning)
            {
                currentCount += _logCounters[PortableConsoleLogType.Warning];
            }

            _logCounters[currentAdded.LogType]++;

            if ((_logFilter & currentAdded.LogType) == currentAdded.LogType)
            {
                draw = true;
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
                if ((_logFilter & l.LogType) != l.LogType)
                {
                    continue;
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
            bg.color = ((counter & 1) == 0) ? Resources.FirstColor : Resources.SecondColor;

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
            InfoCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Info]);
            WarningCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Warning]);
            ErrorCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Error]);
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