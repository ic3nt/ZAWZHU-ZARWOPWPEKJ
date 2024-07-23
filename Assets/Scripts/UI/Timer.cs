using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;

    public TextMeshProUGUI totaltimer;

    public HealthManager hp;

   

    void Update()
    {
        timer.text = FormatTime(Time.time);
        totaltimer.text = FormatTime(Time.time);

        if (hp.timerOn == false)
        {
            StopTimer();
        }

    }

    string FormatTime(float time)
    {
       
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }

    public void StopTimer()
    {
        enabled = false;
    }

}