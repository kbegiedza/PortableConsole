using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PortableConsole
{
    /// <summary>
    /// Main class for PortableConsole plugin
    /// </summary>
    public class PortableConsole : MonoBehaviour
    {
        private static string _defaultEventSystemName = "DefaultEventSystem";

        private readonly Dictionary<PortableConsoleLogType, uint> _logCounters = new Dictionary<PortableConsoleLogType, uint>
        {
            { PortableConsoleLogType.Info, 0},
            { PortableConsoleLogType.Warning, 0},
            { PortableConsoleLogType.Error, 0},
        };

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
        private ScrollRect _mainScrollRect;
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

        //------------------------------
        // Unity methods
        //------------------------------
        private void Awake()
        {
            SearchAndSetupComponents();

            //prepare logs pool
            _logsPool = new GameObjectPool(_logTemplate, _logContainer, 301);
        }

        private void OnEnable()
        {
            //attach our logger to Unity's event
            Application.logMessageReceived += OnLogMessageReceived;

            UpdateLogCount();
        }

        private void OnDisable()
        {
            //detach our logger from Unity's event
            Application.logMessageReceived -= OnLogMessageReceived;

            UpdateLogCount();
        }

        private void OnDestroy()
        {
            //make sure that logger is detached from Unity's event
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        
        //------------------------------
        // public methods
        //------------------------------
        public void OnClickClearButton()
        {
            ClearDisplayedLogs();

            _logs.Clear();
            foreach (var key in _logCounters.Keys.ToList())
            {
                _logCounters[key] = 0;
            }

            UpdateLogCount();
        }

        public void OnClickInfoButton(Image image)
        {
            ToggleLogTypeButton(image, PortableConsoleLogType.Info);
        }

        public void OnClickWarningButton(Image image)
        {
            ToggleLogTypeButton(image, PortableConsoleLogType.Warning);
        }

        public void OnClickErrorButton(Image image)
        {
            ToggleLogTypeButton(image, PortableConsoleLogType.Error);
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
        /// <summary>
        /// Toggles filter and given image based on filter's flags change.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        private void ToggleLogTypeButton(Image image, PortableConsoleLogType type)
        {
            image.color = (ChangeFilter(type)) ? Color.white : Color.gray;
        }

        /// <summary>
        /// Toggles console visibility
        /// </summary>
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

        /// <summary>
        /// Changes filter - updates filer's flags.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool ChangeFilter(PortableConsoleLogType type)
        {
            if ((_logFilter & type) == type)
            {
                _logFilter = _logFilter & ~type;

                RedrawConsoleLogs();

                return false;
            }
            else
            {
                _logFilter = (_logFilter | type);

                RedrawConsoleLogs();

                return true;
            }
        }
    
        /// <summary>
        /// Searches and prepare all required components.
        /// </summary>
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
            _mainScrollRect = _consoleContent.transform.Find("MainContent").GetComponent<ScrollRect>();
            _logContainer = _mainScrollRect.content;

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
            _toggleButton.gameObject.SetActive(true);

            //attach our logger to Unity's event
            Application.logMessageReceived += OnLogMessageReceived;

            UpdateLogCount();
        }

        /// <summary>
        /// Shows console.
        /// </summary>
        private void ShowConsoleContent()
        {
            //show console
            _consoleContent.SetActive(true);

            RedrawConsoleLogs();

            //hide toggle button and notifications
            _notification.SetActive(false);
            _toggleButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides console.
        /// </summary>
        private void HideConsoleContent()
        {
            //show console
            _consoleContent.SetActive(false);

            //hide button
            _toggleButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Callback for Unity's <see cref="Application.logMessageReceived"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
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
                _mainScrollRect.verticalNormalizedPosition = 0;
            }

            ShowInGameNotification(currentLog);

            UpdateLogCount();
        }

        /// <summary>
        /// Clears displayed logs
        /// </summary>
        private void ClearDisplayedLogs()
        {
            foreach (Transform t in _logContainer.transform)
            {
                t.gameObject.SetActive(false);
            }
            _logContainer.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// Clears and redraws all logs.
        /// </summary>
        private void RedrawConsoleLogs()
        {
            ClearDisplayedLogs();

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

        /// <summary>
        /// Draws given <see cref="PortableConsoleLog"/>.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="counter"></param>
        private void DrawConsoleLog(PortableConsoleLog log, uint counter)
        {
            //create instance
            var newLog = _logsPool.GetGameObject().GetComponent<RectTransform>();
            newLog.transform.SetParent(_logContainer.transform);

            var button = newLog.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => 
            {
                ShowStackTrace(log);
            });

            //set background color
            var bg = newLog.GetComponent<Image>();
            bg.color = ((counter & 1) == 0) ? _resources.DefaultStyle.FirstColor : _resources.DefaultStyle.SecondColor;

            //set correspond image
            var icon = newLog.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = _resources.GetIconSpriteFromLogType(log.LogType);

            //update text content
            var logContent = newLog.transform.Find("Content").GetComponent<Text>();
            var logCaster = newLog.transform.Find("Caster").GetComponent<Text>();

            logContent.text = log.Name;
            logCaster.text = log.Caster;

            //manage item's position and size
            var offset = (newLog.sizeDelta / 2f) + newLog.sizeDelta * counter;
            newLog.anchoredPosition = Vector2.zero;
            newLog.anchoredPosition -= offset;

            //show gameobject
            newLog.gameObject.SetActive(true);

            //update scroll rect's content size
            _logContainer.sizeDelta += newLog.sizeDelta;
        }

        /// <summary>
        /// Shows notification on overlay screen.
        /// </summary>
        /// <param name="log"></param>
        private void ShowInGameNotification(PortableConsoleLog log)
        {
            if(_consoleContent.activeInHierarchy)
            {
                return;
            }

            _notification.gameObject.SetActive(true);
            _notificationImage.color = _resources.GetColorFromLogType(log.LogType);
            _notificationText.text = log.Name;
        }

        /// <summary>
        /// Updates logs' text components.
        /// </summary>
        private void UpdateLogCount()
        {
            InfoCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Info]);
            WarningCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Warning]);
            ErrorCounter.text = GetCounterText(_logCounters[PortableConsoleLogType.Error]);
        }

        /// <summary>
        /// Returns corresponding log's count text to display.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Shows stack trace for given <see cref="PortableConsoleLog"/>.
        /// </summary>
        /// <param name="log"></param>
        private void ShowStackTrace(PortableConsoleLog log)
        {
            //update stack trace panel's content
            _stackTraceIcon.sprite = _resources.GetIconSpriteFromLogType(log.LogType);
            _stackTraceContentText.text = string.Format("{0}\n{1}",log.Name,log.Details);
            _stackTraceContent.sizeDelta = new Vector3(_stackTraceContent.sizeDelta.x, _stackTraceContentText.preferredHeight);
            _stackTraceName.text = log.Name;

            _stackTrace.SetActive(true);
        }
    }
}