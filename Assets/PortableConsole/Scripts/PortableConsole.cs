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

        public bool ScrollLocked = false;

        [SerializeField]
        private PortableConsoleResources _resources;
        [SerializeField]
        private GameObject _logTemplate;
        [SerializeField]
        private Button _toggleButton;
        [SerializeField]
        private bool _useDefaultButton = true;

        private RectTransform _logContainer;
        private GameObject _consoleContent;
        private Text InfoCounter;
        private Text WarningCounter;
        private Text ErrorCounter;

        private GameObject _stackTrace;
        private Image _stackTraceIcon;
        private RectTransform _stackTraceContent;
        private InputField _stackTraceContentText;
        private Text _stackTraceName;

        private GameObject _notification;
        private Text _notificationText;
        private Image _notificationImage;

        private GameObjectPool _logsPool;
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
            SearchAndSetupComponents();

            //prepare logs pool
            _logsPool = new GameObjectPool(_logTemplate, _logContainer, 301);
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
            foreach (Transform child in _logContainer.transform)
            {
                child.gameObject.SetActive(false);
            }

            _logContainer.sizeDelta = Vector2.zero;

            _logs.Clear();
            foreach (var key in _logCounters.Keys.ToList())
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

        public void OnScrollLockButton(Image scrollImage)
        {
            ScrollLocked = !ScrollLocked;

            scrollImage.sprite = ScrollLocked ? 
                _resources.DefaultStyle.ScrollLocked 
                : _resources.DefaultStyle.ScrollUnlocked;
        }

        public void OnStackTraceCopyButton()
        {
            GUIUtility.systemCopyBuffer = _stackTraceContentText.text;
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
    
        private void SearchAndSetupComponents()
        {
            //check for EventSystem and create if required
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject obj = new GameObject(_defaultEventSystemName);
                obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>();
            }

            if (_resources == null)
            {
                throw new System.NullReferenceException("PortableConsoleResources is null!");
            }

            _consoleContent = transform.Find("Canvas").Find("Console").gameObject;
            _logContainer = _consoleContent.transform.Find("MainContent").Find("Viewport").Find("Content").GetComponent<RectTransform>();

            var firstTopBar = _consoleContent.transform.Find("TopBar").Find("FirstTopBar");
            InfoCounter = firstTopBar.Find("ToggleInfoButton").Find("Counter").GetComponent<Text>();
            WarningCounter = firstTopBar.Find("ToggleWarningButton").Find("Counter").GetComponent<Text>();
            ErrorCounter = firstTopBar.Find("ToggleErrorButton").Find("Counter").GetComponent<Text>();

            //setup stack trace objects
            _stackTrace = _consoleContent.transform.Find("StackTrace").gameObject;
            _stackTraceIcon = _stackTrace.transform.Find("TopBar").Find("Icon").GetComponent<Image>();
            _stackTraceName = _stackTrace.transform.Find("TopBar").Find("Name").GetComponent<Text>();
            _stackTraceContent = _stackTrace.transform.Find("ScrollRect").Find("Viewport").Find("Content").GetComponent<RectTransform>();
            _stackTraceContentText = _stackTraceContent.transform.Find("TextField").GetComponent<InputField>();

            //setup notification
            _notification = transform.Find("Canvas").Find("Notification").gameObject;
            _notificationImage = _notification.transform.Find("NotificationImage").GetComponent<Image>();
            _notificationText = _notificationImage.transform.Find("NotificationText").GetComponent<Text>();

            //enable click on notification to open console
            _notification.GetComponent<Button>().onClick.AddListener(ToggleContent);

            //add toggle to toggle Button
            _toggleButton.onClick.AddListener(ToggleContent);

            //attach our logger to Unity's event
            Application.logMessageReceived += LogMessageReceived;

            UpdateLogCount();
        }

        private void ShowConsoleContent()
        {
            //show console
            _consoleContent.SetActive(true);

            RedrawConsoleLogs();

            //hide toggle button and notifications
            _notification.SetActive(false);
            _toggleButton.gameObject.SetActive(false);
        }

        private void HideConsoleContent()
        {
            //show console
            _consoleContent.SetActive(false);

            //hide button
            _toggleButton.gameObject.SetActive(true);
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            var currentLog = new PortableConsoleLog(type, condition, stackTrace);

            _logs.Add(currentLog);

            uint currentCount = 0;

            foreach (var key in _logCounters.Keys)
            {
                if ((_logFilter & key) == key)
                {
                    currentCount += _logCounters[key];
                }
            }

            _logCounters[currentLog.LogType]++;

            if ((_logFilter & currentLog.LogType) == currentLog.LogType 
                && _consoleContent.activeInHierarchy)
            {
                DrawConsoleLog(currentLog, currentCount);
            }

            if(!ScrollLocked)
            {
                _logContainer.anchoredPosition = new Vector2(_logContainer.anchoredPosition.x, 100 * _logs.Count);
            }

            //TODO: if show
            ShowInGameNotification(currentLog);

            UpdateLogCount();
        }

        private void RedrawConsoleLogs()
        {
            foreach (Transform t in _logContainer.transform)
            {
                t.gameObject.SetActive(false);
            }
            _logContainer.sizeDelta = Vector2.zero;

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
            var g = _logsPool.GetGameObject().GetComponent<RectTransform>();
            g.transform.SetParent(_logContainer.transform);

            var button = g.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => 
            {
                ShowStackTrace(log);
            });

            //set background color
            var bg = g.GetComponent<Image>();
            bg.color = ((counter & 1) == 0) ? _resources.DefaultStyle.FirstColor : _resources.DefaultStyle.SecondColor;

            //set correspond image
            var icon = g.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = _resources.GetLogTypeIconSprite(log.LogType);

            //update text content
            var logContent = g.transform.Find("Content").GetComponent<Text>();
            var logCaster = g.transform.Find("Caster").GetComponent<Text>();

            logContent.text = log.Name;
            logCaster.text = log.Caster;

            //manage item's position and size
            var size = new Vector2(0, 100);
            var offset = (size / 2f) + size * counter;

            g.anchoredPosition = Vector2.zero;

            g.sizeDelta = size;
            g.anchoredPosition -= offset;

            //show gameobject
            g.gameObject.SetActive(true);

            //update scroll rect's content size
            _logContainer.sizeDelta += size;
        }

        private void ShowInGameNotification(PortableConsoleLog log)
        {
            if(_consoleContent.activeInHierarchy)
            {
                return;
            }

            _notification.gameObject.SetActive(true);
            _notificationImage.color = _resources.GetLogTypeColor(log.LogType);
            _notificationText.text = log.Name;
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

        private void ShowStackTrace(PortableConsoleLog log)
        {
            _stackTraceIcon.sprite = _resources.GetLogTypeIconSprite(log.LogType);
            _stackTraceContentText.text = string.Format("{0}\n{1}",log.Name,log.Details);
            _stackTraceContent.sizeDelta = new Vector3(_stackTraceContent.sizeDelta.x, _stackTraceContentText.preferredHeight);
            _stackTraceName.text = log.Name;

            _stackTrace.SetActive(true);
        }
        //------------------------------
        // coroutines
        //------------------------------
    }
}