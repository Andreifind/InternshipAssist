using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ComboBar : MonoBehaviour
{
    public Slider TimerSlider;
    public float MaxSeconds;
    private float _timeLeft;
    private bool _timerStarted;
    public int ComboLevel = 0;
    public TextMeshProUGUI Text;

    private void Start()
    {
        _timerStarted = false;
        TimerSlider.value = 0;
    }

    public void AddCombo()
    {
        if (_timeLeft > 0f)
            ComboLevel++;
        else
            ComboLevel = 1;
        _timeLeft = MaxSeconds - ComboLevel / 2;
        TimerSlider.maxValue = _timeLeft;
        _timerStarted = true;
        Text.text = $"Combo x {ComboLevel}";

    }

    public void ResetCombo()
    {
        _timeLeft = 0;
    }

    private void Update()
    {
        if(_timerStarted)
        {
            if(_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                TimerSlider.value = _timeLeft;
            }
            else
            {
                TimerSlider.value = 0;
                _timerStarted = false;
                Text.text = " ";
            }
        }
    }
}
