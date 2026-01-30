using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Time_UI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    private void OnEnable() 
    {
        TimeManager.OnDateTimeChanged += UpdateTime;
    }

    private void OnDisable() 
    {
        TimeManager.OnDateTimeChanged -= UpdateTime;
    }

    private void UpdateTime(TimeManager.DateTime dateTime)
    {
        timeText.text = dateTime.TimeToString();

        if (dateText != null)
        {
            dateText.text = dateTime.DateToString();
        }
        
    }
}
