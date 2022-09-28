using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    #region Exposed

    [SerializeField] private TimerMode _mode;
    [SerializeField] private TMPro.TMP_Text _display;
    [SerializeField] private float _timeLimit;
    [SerializeField] private UnityEvent<float> _onTimeLimitReached;

    #endregion

    
    #region Properties

    public bool Visible => _display.enabled;
    public float Time => _time;
    public TimerMode Mode => _mode;
    public float TimeLimit => _timeLimit;
        
    #endregion

    
    #region Unity API

    private void Update()
    {
        if (!_isRunning) return;
        HandleTime();
        UpdateDisplay();
    }

    #endregion

    
    #region Main

    public void Play()
    {
        if(_isRunning) return;

        Reset();
        _isRunning = true;
    }

    public void Resume()
    {
        if(_isRunning || _time == 0) return;

        _isRunning = true;
    }

    public void Stop()
    {
        if(!_isRunning) return;

        _isRunning = false;
    }

    public void Reset()
    {
        _time = _mode switch 
        {
            TimerMode.Timer => _timeLimit,
            _ => 0
        }; 

        UpdateDisplay();
    }

    public void AddTime(float timeAmount)
    {
        _time += timeAmount;
    }

    #endregion


    #region Utils

    public void SetMode(TimerMode mode)
    {
        _mode = mode;
    }

    public void SetTimerLimit(float timeLimit)
    {
        _timeLimit = Mathf.Max(0, timeLimit);
    }

    public void SetVisible(bool visibility)
    {
        _display.enabled = visibility;
    }

    #endregion

    
    #region Plumbery

    private void HandleTime()
    {
        if (_mode == TimerMode.Chrono)
        {
            _time += UnityEngine.Time.deltaTime;
        }

        else if (_mode == TimerMode.Timer)
        {
            _time -= UnityEngine.Time.deltaTime;
            if (_time <= 0)
            {
                _time = 0;
                Stop();
                _onTimeLimitReached?.Invoke(_time);
            }
        }
    }

    private void UpdateDisplay()
    {
        var minute = Mathf.FloorToInt(_time / 60);
        var second = _time - minute * 60;
        _display.text = $"{(minute > 0 ? minute + " : " : "")}{second : #0.00}";
    }
        
    #endregion


    #region Private Fields

    private float _time;
    private bool _isRunning;

    #endregion


    public enum TimerMode
    {
        Timer,
        Chrono
    }
}

