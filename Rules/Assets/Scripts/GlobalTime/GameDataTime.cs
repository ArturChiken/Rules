using UnityEngine;
using System;

/// <summary>
/// Полная система даты и времени для игры
/// </summary>
public class GameDateTime : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ ВРЕМЕНИ ===")]
    [SerializeField] private bool enableDateSystem = true;     // Включить систему дат
    [SerializeField] private bool enableYear = true;           // Показывать год
    [SerializeField] private bool enableMonth = true;          // Показывать месяц
    [SerializeField] private bool enableDay = true;            // Показывать день
    [SerializeField] private bool enableHour = true;           // Показывать часы
    [SerializeField] private bool enableMinute = true;         // Показывать минуты
    [SerializeField] private bool enableSecond = false;        // Показывать секунды

    [Header("=== СКОРОСТЬ ВРЕМЕНИ ===")]
    [SerializeField] private float timeScale = 60f;            // 1 сек реальная = 1 мин игровая
    [SerializeField] private bool useRealtimeForDebug = false; // Для отладки

    [Header("=== НАЧАЛЬНЫЕ ЗНАЧЕНИЯ ===")]
    [SerializeField] private int startYear = 2024;
    [SerializeField] private int startMonth = 1;               // 1-12
    [SerializeField] private int startDay = 1;                // 1-31
    [SerializeField] private int startHour = 0;               // 0-23
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int startSecond = 0;

    [Header("=== ДЛИТЕЛЬНОСТЬ СУТОК ===")]
    [SerializeField] private float dayLengthHours = 24f;      // Можно менять во время игры

    [Header("=== НАСТРОЙКИ МЕСЯЦЕВ ===")]
    [SerializeField] private int daysInMonth = 30;             // По умолчанию 30 дней в месяце
    [SerializeField] private bool useRealMonths = false;       // Использовать реальные месяцы (28-31 день)

    [Header("=== ОТЛАДКА ===")]
    [SerializeField] private bool logTimeChanges = false;
    [SerializeField] private bool logDateChanges = false;

    // ========== ТЕКУЩИЕ ЗНАЧЕНИЯ ==========
    private float currentTimeInSeconds;  // Текущее время в секундах от начала суток

    private int currentYear;
    private int currentMonth;
    private int currentDay;
    private int currentHour;
    private int currentMinute;
    private int currentSecond;

    // ========== СОБЫТИЯ ДЛЯ ДРУГИХ СИСТЕМ ==========
    public System.Action OnDateTimeChanged;           // Любое изменение времени/даты
    public System.Action OnHourChanged;               // Изменился час
    public System.Action OnDayChanged;                // Начался новый день
    public System.Action OnMonthChanged;              // Начался новый месяц
    public System.Action OnYearChanged;               // Начался новый год
    public System.Action<float> OnTimeScaleChanged;   // Изменилась скорость времени

    // ========== СВОЙСТВА ДЛЯ ДОСТУПА ==========
    public int Year => currentYear;
    public int Month => currentMonth;
    public int Day => currentDay;
    public int Hour => currentHour;
    public int Minute => currentMinute;
    public int Second => currentSecond;
    public float DayLengthHours => dayLengthHours;
    public float TimeScale => timeScale;

    // Нормализованное время дня (0-1)
    public float NormalizedTimeOfDay => currentTimeInSeconds / (dayLengthHours * 3600f);

    // Текущая дата и время в разных форматах
    public string DateTimeString => GetDateTimeString();
    public string TimeString => GetTimeString();
    public string DateString => GetDateString();

    // Singleton
    public static GameDateTime Instance { get; private set; }

    // ========== МАССИВЫ ДЛЯ РЕАЛЬНЫХ МЕСЯЦЕВ ==========
    private readonly int[] daysInRealMonths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDateTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDateTime()
    {
        currentYear = startYear;
        currentMonth = Mathf.Clamp(startMonth, 1, 12);
        currentDay = Mathf.Clamp(startDay, 1, GetDaysInMonth(currentYear, currentMonth));
        currentHour = Mathf.Clamp(startHour, 0, 23);
        currentMinute = Mathf.Clamp(startMinute, 0, 59);
        currentSecond = Mathf.Clamp(startSecond, 0, 59);

        currentTimeInSeconds = (currentHour * 3600f) + (currentMinute * 60f) + currentSecond;

        if (logTimeChanges)
            Debug.Log($"Дата и время инициализированы: {DateTimeString}");
    }

    private void Update()
    {
        if (useRealtimeForDebug)
        {
            // Для отладки: синхронизация с реальным временем
            var now = DateTime.Now;
            currentYear = now.Year;
            currentMonth = now.Month;
            currentDay = now.Day;
            currentHour = now.Hour;
            currentMinute = now.Minute;
            currentSecond = now.Second;
            currentTimeInSeconds = (currentHour * 3600f) + (currentMinute * 60f) + currentSecond;
        }
        else
        {
            // Обновляем время
            float deltaSeconds = Time.deltaTime * timeScale;
            currentTimeInSeconds += deltaSeconds;

            // Проверяем, не прошел ли час
            int oldHour = currentHour;
            int oldDay = currentDay;
            int oldMonth = currentMonth;
            int oldYear = currentYear;

            // Конвертируем секунды в часы/минуты/секунды
            float totalHours = currentTimeInSeconds / 3600f;

            if (totalHours >= dayLengthHours)
            {
                // Новый день!
                currentTimeInSeconds -= dayLengthHours * 3600f;
                totalHours = currentTimeInSeconds / 3600f;

                // Увеличиваем день
                currentDay++;

                // Проверяем, не закончился ли месяц
                int daysInCurrentMonth = GetDaysInMonth(currentYear, currentMonth);
                if (currentDay > daysInCurrentMonth)
                {
                    currentDay = 1;
                    currentMonth++;

                    if (currentMonth > 12)
                    {
                        currentMonth = 1;
                        currentYear++;
                    }

                    if (logDateChanges && enableMonth)
                        Debug.Log($"Новый месяц: {GetMonthName()} {currentYear}");
                }

                if (logDateChanges && enableDay)
                    Debug.Log($"Новый день: {currentDay}.{currentMonth}.{currentYear}");
            }

            // Обновляем часы, минуты, секунды
            currentHour = Mathf.FloorToInt(totalHours);
            currentMinute = Mathf.FloorToInt((totalHours % 1) * 60);
            currentSecond = Mathf.FloorToInt(currentTimeInSeconds % 60);

            // Вызываем события при изменениях
            if (oldHour != currentHour && enableHour)
            {
                OnHourChanged?.Invoke();
                if (logTimeChanges)
                    Debug.Log($"Час: {currentHour}:00");
            }

            if (oldDay != currentDay && enableDay)
                OnDayChanged?.Invoke();

            if (oldMonth != currentMonth && enableMonth)
                OnMonthChanged?.Invoke();

            if (oldYear != currentYear && enableYear)
                OnYearChanged?.Invoke();
        }

        OnDateTimeChanged?.Invoke();
    }

    // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

    private int GetDaysInMonth(int year, int month)
    {
        if (!useRealMonths)
            return daysInMonth;

        int days = daysInRealMonths[month - 1];

        // Учет високосного года для февраля
        if (month == 2 && IsLeapYear(year))
            return 29;

        return days;
    }

    private bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    private string GetMonthName()
    {
        string[] monthNames = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                                "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        return monthNames[currentMonth - 1];
    }

    // ========== МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ==========

    /// <summary>
    /// Установить скорость времени
    /// </summary>
    public void SetTimeScale(float scale)
    {
        timeScale = Mathf.Max(0, scale);
        OnTimeScaleChanged?.Invoke(timeScale);
        if (logTimeChanges)
            Debug.Log($"Скорость времени: {timeScale}x");
    }

    /// <summary>
    /// Установить длительность суток (в часах)
    /// </summary>
    public void SetDayLength(float hours)
    {
        if (hours <= 0) return;

        float normalizedTime = NormalizedTimeOfDay;
        dayLengthHours = hours;
        currentTimeInSeconds = normalizedTime * (dayLengthHours * 3600f);

        // Обновляем отображение
        float totalHours = currentTimeInSeconds / 3600f;
        currentHour = Mathf.FloorToInt(totalHours);
        currentMinute = Mathf.FloorToInt((totalHours % 1) * 60);
        currentSecond = Mathf.FloorToInt(currentTimeInSeconds % 60);

        if (logTimeChanges)
            Debug.Log($"Длина суток изменена: {dayLengthHours} часов");
    }

    /// <summary>
    /// Установить конкретное время
    /// </summary>
    public void SetTime(int hour, int minute, int second)
    {
        currentHour = Mathf.Clamp(hour, 0, 23);
        currentMinute = Mathf.Clamp(minute, 0, 59);
        currentSecond = Mathf.Clamp(second, 0, 59);
        currentTimeInSeconds = (currentHour * 3600f) + (currentMinute * 60f) + currentSecond;
        OnDateTimeChanged?.Invoke();

        if (logTimeChanges)
            Debug.Log($"Время установлено: {GetTimeString()}");
    }

    /// <summary>
    /// Установить конкретную дату
    /// </summary>
    public void SetDate(int year, int month, int day)
    {
        currentYear = year;
        currentMonth = Mathf.Clamp(month, 1, 12);
        currentDay = Mathf.Clamp(day, 1, GetDaysInMonth(year, month));
        OnDateTimeChanged?.Invoke();

        if (logDateChanges)
            Debug.Log($"Дата установлена: {GetDateString()}");
    }

    /// <summary>
    /// Добавить время
    /// </summary>
    public void AddTime(int hours, int minutes, int seconds)
    {
        float addSeconds = (hours * 3600f) + (minutes * 60f) + seconds;
        currentTimeInSeconds += addSeconds;

        while (currentTimeInSeconds >= dayLengthHours * 3600f)
        {
            currentTimeInSeconds -= dayLengthHours * 3600f;
            currentDay++;

            int daysInMonth = GetDaysInMonth(currentYear, currentMonth);
            if (currentDay > daysInMonth)
            {
                currentDay = 1;
                currentMonth++;
                if (currentMonth > 12)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }
        }

        // Обновляем часы/минуты/секунды
        float totalHours = currentTimeInSeconds / 3600f;
        currentHour = Mathf.FloorToInt(totalHours);
        currentMinute = Mathf.FloorToInt((totalHours % 1) * 60);
        currentSecond = Mathf.FloorToInt(currentTimeInSeconds % 60);

        OnDateTimeChanged?.Invoke();

        if (logTimeChanges)
            Debug.Log($"Добавлено время: +{hours}:{minutes:00}:{seconds:00}");
    }

    /// <summary>
    /// Добавить дни
    /// </summary>
    public void AddDays(int days)
    {
        for (int i = 0; i < days; i++)
        {
            currentDay++;
            int daysInMonth = GetDaysInMonth(currentYear, currentMonth);
            if (currentDay > daysInMonth)
            {
                currentDay = 1;
                currentMonth++;
                if (currentMonth > 12)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }
        }
        OnDateTimeChanged?.Invoke();

        if (logDateChanges)
            Debug.Log($"Добавлено дней: +{days}");
    }

    /// <summary>
    /// Получить строку времени (HH:MM:SS)
    /// </summary>
    public string GetTimeString()
    {
        if (!enableHour) return "";

        string time = "";
        if (enableHour) time += $"{currentHour:00}";
        if (enableMinute) time += $":{currentMinute:00}";
        if (enableSecond) time += $":{currentSecond:00}";

        return time;
    }

    /// <summary>
    /// Получить строку даты (ДД.ММ.ГГГГ)
    /// </summary>
    public string GetDateString()
    {
        string date = "";
        if (enableDay) date += $"{currentDay:00}";
        if (enableMonth) date += $".{currentMonth:00}";
        if (enableYear) date += $".{currentYear}";

        return date;
    }

    /// <summary>
    /// Получить полную строку даты и времени
    /// </summary>
    public string GetDateTimeString()
    {
        string result = "";
        if (enableDateSystem)
        {
            result += GetDateString();
            if (enableHour) result += " ";
        }
        result += GetTimeString();
        return result;
    }

    /// <summary>
    /// Для отладки - показать текущее состояние
    /// </summary>
    public void PrintCurrentDateTime()
    {
        Debug.Log($"=== ТЕКУЩАЯ ДАТА И ВРЕМЯ ===");
        Debug.Log($"Дата: {GetDateString()}");
        Debug.Log($"Время: {GetTimeString()}");
        Debug.Log($"Скорость времени: {timeScale}x");
        Debug.Log($"Длина дня: {dayLengthHours} часов");
    }
}