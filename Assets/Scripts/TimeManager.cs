using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [Header("Date & Time Settings")]
    [Range(1, 28)]
    public int dateInMonth;
    [Range(1, 4)]
    public int season;
    [Range(1, 99)]
    public int year;
    [Range(6, 22)]
    public int hour;
    [Range(0, 6)]
    public int minute;

    private DateTime dateTime;
    [Header("Tick Settings")]
    public int TickMinutesIncrease = 10;
    public float TimeBetweenTicks = 1f;
    private float currentTimeBetweenTicks;

    private float passOutTimer;
    private const float passOutDuration = 120f;

    public TimeState currentState { get; private set;} = TimeState.Normal;

    public static UnityAction<DateTime> OnDateTimeChanged;

    // 20 real minute = 1 in-game day
    
    private void Awake() 
    {
        dateTime = new DateTime(dateInMonth, season - 1, year, hour, minute);

    }
    void Start()
    {
        OnDateTimeChanged?.Invoke(dateTime);
    }

    
    void Update()
    {
        if (currentState == TimeState.Frozen || currentState == TimeState.Sleeping)
            return;

        if (dateTime.IsCollapseHour())
        {
            StartPassOutCountdown();
            return;
        }

        if (currentState == TimeState.Countdown)
        {
            UpdateCountdown();
            return;
        }

        currentTimeBetweenTicks += Time.deltaTime;
        if (currentTimeBetweenTicks >= TimeBetweenTicks)
        {
            currentTimeBetweenTicks = 0f;
            Tick();
        }
    }

    void Tick()
    {
        AdvanceTime();
    }

    void AdvanceTime()
    {
        dateTime.AdvanceMinutes(TickMinutesIncrease);

        OnDateTimeChanged?.Invoke(dateTime);
    }

    void StartPassOutCountdown()
    {
        currentState = TimeState.Countdown;
        passOutTimer = passOutDuration;
    }

    void UpdateCountdown()
    {
        passOutTimer -= Time.deltaTime;

        if (passOutTimer <= 0f)
        {
            PassOut();
        }
    }

    void PassOut()
    {
        currentState = TimeState.Sleeping;

        dateTime = new DateTime(
            dateTime.Date + 1,
            (int)dateTime.Season,
            dateTime.Year,
            9,
            0
        );

        GameManager.instance.tileManager.OnDayPassed();
        OnDateTimeChanged?.Invoke(dateTime);
    }

    public void Sleep()
    {
        currentState = TimeState.Sleeping;

        dateTime = new DateTime(
            dateTime.Date + 1,
            (int)dateTime.Season,
            dateTime.Year,
            6,
            0
        );

        GameManager.instance.tileManager.OnDayPassed();
        OnDateTimeChanged?.Invoke(dateTime);
    }

    public void WakeUp()
    {
        currentState = TimeState.Normal;
    }

    [System.Serializable]
    public struct DateTime
    {
        #region Fields
        private Days day;
        private int date;
        private int year;
        
        private int hour;
        private int minutes;

        private Season season;

        private int totalNumDays;
        private int totalNumWeeks;

        #endregion

        #region Properties
        public Days Day => day;
        public int Date => date;
        public int Hour => hour;
        public Season Season => season;
        public int Year => year;
        public int TotalNumDays  => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;
        #endregion

        #region Constructor

        public DateTime(int date, int season, int year, int hour, int minutes)
        {
            this.day = (Days)(date % 7);
            if (day == 0) day = (Days)7;
            this.date = date;
            this.season = (Season)season;
            this.year = year;

            this.hour = hour;
            this.minutes = minutes;

            totalNumDays = date + (28 * (int)this.season) + (112 * (year - 1));

            totalNumWeeks = 1 + totalNumDays / 7;
        }

        #endregion

        #region  Time Advancement

        public void AdvanceMinutes(int SecondToAdvanceBy)
        {
            if (minutes + SecondToAdvanceBy >= 60)
            {
                minutes = (minutes + SecondToAdvanceBy) % 60;
                AdvanceHours();
            }
            else
            {
                minutes += SecondToAdvanceBy;
            }
        }

        private void AdvanceHours()
        {
            if ((hour + 1) == 24)
            {
                hour = 0;
                AdvanceDay();
            }
            else
            {
                hour++;
            }
        }

        private void AdvanceDay()
        {
            day++;
        
            if (day + 1 > (Days)7)
            {
                day = (Days)1;
                totalNumWeeks++;
            }

            date++;
            
            if (date % 29 == 0)
            {
                AdvanceSeason();
                date = 1;
            }

            totalNumDays++;
        }

        private void AdvanceSeason()
        {
            if ((int)season + 1 > 4)
            {
                season = (Season)1;
                AdvanceYear();
            }
            else season++;
        }

        private void AdvanceYear()
        {
            date = 1;
            year++;
        }

        #endregion

        #region Bool Checks
        public bool IsCollapseHour()
        {
            return hour == 22;
        }
        public bool IsNight()
        {
            return hour  > 18 || hour < 6;
        }

        public bool IsMorning()
        {
            return hour >= 6 && hour <= 12;
        }

        public bool IsAfternoon()
        {
            return hour > 12 && hour <= 18;
        }

        public bool IsWeekend()
        {
            return day > Days.Fri ? true : false;
        }

        public bool IsParticularDay(Days _day)
        {
            return day == _day;
        }
        #endregion

        #region  Key Dates
        
        public DateTime NewYearsDay(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(1, 0, year, 6, 0);
        }

        #endregion

        #region Start Of Season

        

        #endregion

        #region To Strings

        public override string ToString()
        {
            return $"Date: {DateToString()} Season: {season.ToString()} TIme: {TimeToString()}" +
                     $" \nTotal Days: {totalNumDays} | Total Weeks: {totalNumWeeks}";
        }

        public string DateToString()
        {
            return $"{Day}, {Date.ToString("D2")} {Season.ToString()} {Year.ToString("D2")}";
        }

        public string TimeToString()
        {
            return $"{hour.ToString("D2")}:{minutes.ToString("D2")}";
        }

        #endregion
    }

    [System.Serializable]
    public enum Days
    {
        NULL = 0,
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
    }
    [System.Serializable]
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Fall = 2,
        Winter = 3
    }

    public enum TimeState
    {
        Normal,
        Frozen,
        Countdown,
        Sleeping
    }
}
