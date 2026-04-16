using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WatchUI : MonoBehaviour
{
    [Header("=== ДИСПЛЕИ ===")]
    [SerializeField] private TextMeshProUGUI dayOfWeekText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image timerIcon;

    [Header("=== СЕКУНДОМЕР ===")]
    [SerializeField] private TextMeshProUGUI stopwatchText;

    [Header("=== ТАЙМЕР ===")]
    [SerializeField] private TextMeshProUGUI timerText;

    // Локальные копии значений для отображения
    private float stopwatchTime = 0f;
    private float timerCurrentTime = 0f;
    private bool timerRunning = false;

    private int timerHours = 0;
    private int timerMinutes = 0;
    private int timerSeconds = 0;

    [Header("=== SETTINGS ===")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.green;

    private enum WatchMode { Clock, Stopwatch, Timer }
    private enum TimerInputState { Off, Seconds, Minutes, Hours }

    private WatchMode currentMode = WatchMode.Clock;
    private GameDateTime gameDateTime;
    private PlayerControl playerControl;
    private TimerInputState inputState = TimerInputState.Off;

    private bool timerInputMode = false;
    private string currentInputBuffer = "";
    private int maxDigitsPerField = 2;

    private void Awake()
    {
        playerControl = FindFirstObjectByType<PlayerControl>();
        gameDateTime = GameDateTime.Instance;
    }

    private void Start()
    {
        // Подписываемся на события GameDateTime
        if (gameDateTime != null)
        {
            gameDateTime.OnDateTimeChanged += UpdateClockDisplay;
            gameDateTime.OnStopwatchUpdate += OnStopwatchUpdate;
            gameDateTime.OnTimerUpdate += OnTimerUpdate;
            gameDateTime.OnTimerComplete += OnTimerComplete;
        }

        UpdateAllDisplays();
        UpdateTimerDisplay();
    }

    private void OnEnable()
    {
        if (playerControl != null)
        {
            playerControl.OnWatchLeftClick += OnLeftClick;
            playerControl.OnWatchRightClick += OnRightClick;
            playerControl.OnWatchBackspace += OnBackspace;
            playerControl.OnDigitPressed += OnDigitPressed;

            playerControl.BlockOtherInputs(true);
        }
    }

    private void OnDisable()
    {
        if (playerControl != null)
        {
            playerControl.OnWatchLeftClick -= OnLeftClick;
            playerControl.OnWatchRightClick -= OnRightClick;
            playerControl.OnWatchBackspace -= OnBackspace;
            playerControl.OnDigitPressed -= OnDigitPressed;

            playerControl.BlockOtherInputs(false);
        }
    }

    private void Update()
    {
        // Только обновляем отображение, логика в GameDateTime
        if (!gameObject.activeSelf) return;

        UpdateClockDisplay();
        UpdateStopwatchDisplay();
        UpdateTimerDisplay();
    }

    #region Display Updates
    private void UpdateClockDisplay()
    {
        if (gameDateTime == null) return;

        if (dayOfWeekText != null)
            dayOfWeekText.text = gameDateTime.GetDayOfWeek();
        if (dateText != null)
            dateText.text = gameDateTime.GetShortDate();
        if (timeText != null)
            timeText.text = gameDateTime.GetFormattedTime();
        if (timerIcon != null)
            timerIcon.color = timerRunning ? activeColor : normalColor;
    }

    private void UpdateStopwatchDisplay()
    {
        if (stopwatchText != null && currentMode == WatchMode.Stopwatch)
        {
            int minutes = Mathf.FloorToInt(stopwatchTime / 60f);
            int seconds = Mathf.FloorToInt(stopwatchTime % 60);
            int milliseconds = Mathf.FloorToInt((stopwatchTime * 100f) % 100);
            stopwatchText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        if (timerInputMode)
        {
            // Режим ввода: показываем вводимые значения с подсветкой
            string hoursStr = timerHours.ToString("00");
            string minutesStr = timerMinutes.ToString("00");
            string secondsStr = timerSeconds.ToString("00");

            switch (inputState)
            {
                case TimerInputState.Hours:
                    hoursStr = $"<color=#{ColorUtility.ToHtmlStringRGB(activeColor)}>{hoursStr}</color>";
                    break;
                case TimerInputState.Minutes:
                    minutesStr = $"<color=#{ColorUtility.ToHtmlStringRGB(activeColor)}>{minutesStr}</color>";
                    break;
                case TimerInputState.Seconds:
                    secondsStr = $"<color=#{ColorUtility.ToHtmlStringRGB(activeColor)}>{secondsStr}</color>";
                    break;
            }

            timerText.text = $"{hoursStr}:{minutesStr}:{secondsStr}";
        }
        else if (timerRunning || gameDateTime?.GetTimerCurrentTime() > 0f == true)
        {
            int hours = Mathf.FloorToInt(timerCurrentTime / 3600f);
            int minutes = Mathf.FloorToInt((timerCurrentTime % 3600) / 60f);
            int seconds = Mathf.FloorToInt(timerCurrentTime % 60);
            timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        }
        else
        {
            timerText.text = "00:00:00";
        }
    }
    #endregion

    #region Event Handlers from GameDateTime
    private void OnStopwatchUpdate(float time)
    {
        stopwatchTime = time;
        if (currentMode == WatchMode.Stopwatch && gameObject.activeSelf)
            UpdateStopwatchDisplay();
    }

    private void OnTimerUpdate(float time)
    {
        timerCurrentTime = time;
        timerRunning = gameDateTime?.IsTimerRunning() ?? false;
        if (currentMode == WatchMode.Timer && gameObject.activeSelf)
            UpdateTimerDisplay();
    }

    private void OnTimerComplete()
    {
        Debug.Log("Таймер завершен!");
        timerRunning = false;
        if (gameObject.activeSelf)
            UpdateTimerDisplay();
    }
    #endregion

    #region Input Handling
    private void OnDigitPressed(int digit)
    {
        if (timerInputMode)
        {
            AddDigit(digit);
        }
    }

    private void OnLeftClick()
    {
        switch (currentMode)
        {
            case WatchMode.Clock:
                gameDateTime?.ToggleTimeFormat();
                break;

            case WatchMode.Stopwatch:
                if (gameDateTime != null)
                {
                    if (gameDateTime.IsStopwatchRunning())
                        gameDateTime.StopwatchStop();
                    else if (stopwatchTime > 0f)
                        gameDateTime.StopwatchReset();
                    else
                        gameDateTime.StopwatchStart();
                }
                break;

            case WatchMode.Timer:
                if (timerInputMode)
                {
                    // Если в режиме ввода — запускаем таймер с введенными значениями
                    StartTimerFromCurrentValues();
                    timerInputMode = false;
                }
                else if (timerRunning)
                {
                    gameDateTime?.TimerReset();
                }
                else
                {
                    EnterTimerInputMode();
                }
                break;
        }
    }

    private void OnRightClick()
    {
        if (timerInputMode) return;

        currentMode = (WatchMode)(((int)currentMode + 1) % 3);
        UpdateAllDisplays();
    }

    private void OnBackspace()
    {
        if (timerInputMode)
        {
            if (currentInputBuffer.Length > 0)
            {
                currentInputBuffer = currentInputBuffer.Substring(0, currentInputBuffer.Length - 1);
            }
            else
            {
                ClearCurrentField();
                MoveToPreviousField();
            }
            UpdateTimerDisplay();
        }
    }
    #endregion

    #region Timer Input Logic
    private void EnterTimerInputMode()
    {
        timerInputMode = true;

        // Загружаем текущие значения таймера (если есть)
        if (gameDateTime != null && gameDateTime.GetTimerCurrentTime() > 0f)
        {
            float currentTime = gameDateTime.GetTimerCurrentTime();
            timerHours = Mathf.FloorToInt(currentTime / 3600f);
            timerMinutes = Mathf.FloorToInt((currentTime % 3600) / 60f);
            timerSeconds = Mathf.FloorToInt(currentTime % 60);
        }
        else
        {
            timerHours = 0;
            timerMinutes = 0;
            timerSeconds = 0;
        }

        currentInputBuffer = "";
        inputState = TimerInputState.Seconds;
        UpdateTimerDisplay();
    }

    private void AddDigit(int digit)
    {
        int currentValue = GetCurrentFieldValue();
        int newValue;
        if (currentValue >= 10)
        {
            newValue = (currentValue % 10) * 10 + digit;
        }
        else
        {
            newValue = currentValue * 10 + digit;
        }

        int maxValue = (inputState == TimerInputState.Hours) ? 23 : 59;
        newValue = Mathf.Clamp(newValue, 0, maxValue);

        SetCurrentFieldValue(newValue);

        bool isTwoDigit = newValue >= 10 || (newValue <= 9 && currentValue > 0);

        if (isTwoDigit)
        {
            // Переключаемся на следующее поле
            MoveToNextField();
        }

        UpdateTimerDisplay();
    }

    private int GetCurrentFieldValue()
    {
        switch (inputState)
        {
            case TimerInputState.Seconds: return timerSeconds;
            case TimerInputState.Minutes: return timerMinutes;
            case TimerInputState.Hours: return timerHours;
            default: return 0;
        }
    }

    private void SetCurrentFieldValue(int value)
    {
        switch (inputState)
        {
            case TimerInputState.Seconds: timerSeconds = value; break;
            case TimerInputState.Minutes: timerMinutes = value; break;
            case TimerInputState.Hours: timerHours = value; break;
        }
    }

    private void MoveToNextField()
    {
        switch (inputState)
        {
            case TimerInputState.Seconds:
                inputState = TimerInputState.Minutes;
                break;
            case TimerInputState.Minutes:
                inputState = TimerInputState.Hours;
                break;
            case TimerInputState.Hours:
                break;
        }
        UpdateTimerDisplay();
    }

    private void MoveToPreviousField()
    {
        switch (inputState)
        {
            case TimerInputState.Hours:
                inputState = TimerInputState.Minutes;
                ClearCurrentField();
                break;
            case TimerInputState.Minutes:
                inputState = TimerInputState.Seconds;
                ClearCurrentField();
                break;
            case TimerInputState.Seconds:
                break;
        }
        UpdateTimerDisplay();
    }

    private void ClearCurrentField()
    {
        switch (inputState)
        {
            case TimerInputState.Seconds: timerSeconds = 0; break;
            case TimerInputState.Minutes: timerMinutes = 0; break;
            case TimerInputState.Hours: timerHours = 0; break;
        }
        currentInputBuffer = "";
    }

    private void StartTimerFromCurrentValues()
    {
        float totalSeconds = (timerHours * 3600f) + (timerMinutes * 60f) + timerSeconds;

        if (totalSeconds <= 0f)
        {
            gameDateTime?.TimerReset();
            UpdateTimerDisplay();
            return;
        }

        gameDateTime?.TimerStart(totalSeconds);
        UpdateTimerDisplay();
    }
    #endregion

    #region UI Methods
    public void OpenWatch()
    {
        currentMode = WatchMode.Clock;
        timerInputMode = false;
        UpdateAllDisplays();
        UpdateTimerDisplay();
    }

    public void CloseWatch()
    {
        timerInputMode = false;
    }

    private void UpdateAllDisplays()
    {
        bool isClock = currentMode == WatchMode.Clock;
        bool isStopwatch = currentMode == WatchMode.Stopwatch;
        bool isTimer = currentMode == WatchMode.Timer;

        if (timeText != null) timeText.gameObject.SetActive(isClock);
        if (stopwatchText != null) stopwatchText.gameObject.SetActive(isStopwatch);
        if (timerText != null) timerText.gameObject.SetActive(isTimer);
        if (timerIcon != null) timerIcon.color = timerRunning ? activeColor : normalColor;
    }
    #endregion

    private void OnDestroy()
    {
        if (gameDateTime != null)
        {
            gameDateTime.OnDateTimeChanged -= UpdateClockDisplay;
            gameDateTime.OnStopwatchUpdate -= OnStopwatchUpdate;
            gameDateTime.OnTimerUpdate -= OnTimerUpdate;
            gameDateTime.OnTimerComplete -= OnTimerComplete;
        }
    }
}