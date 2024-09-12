using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public float Duration;
    public float TimeLeft;
    private float _t;
    public float ColorLerpingDuration = 1f;

    private void Start()
    {

    }

    private void Update()
    {
        if(Time.timeScale>0 && TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            Text.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(TimeLeft / 60), Mathf.FloorToInt(TimeLeft % 60));
        }
        if(TimeLeft<30)
        {
            _t = Mathf.PingPong(Time.time / ColorLerpingDuration, 1f);
            Text.color = Color.Lerp(Color.white, Color.red, _t);
        }
    }

}
