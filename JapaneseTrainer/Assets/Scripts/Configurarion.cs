using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public class Configurarion : MonoBehaviour
{
    #region Exposed

    [Header("Global")]
    [SerializeField] private GameConfig _config;
    [SerializeField] private CharacterLibrary _defaultLibrary;
    [SerializeField] private Timer _timer;
    [SerializeField] private TMPro.TMP_Dropdown _gameModeDisplay;
    

    [Header("Character enabling")]
    [SerializeField] private RectTransform _characterToggleContainer;
    [SerializeField] private GridToggle[] _characterToggles;

    
    [Header("Timer")]
    [SerializeField] private Toggle _timerVisibilityToggle;
    [SerializeField] private RectTransform _timerLimitDisplay;
    [SerializeField] private TMPro.TMP_InputField _timeLimitField;

    [Header("Character amount")]
    [SerializeField] private RectTransform _amountDisplay;
    [SerializeField] private TMPro.TMP_InputField _amountField;
        
    #endregion

    
    #region Unity API

    private void Awake()
    {
        _gameModeDisplay.value = (int)_config.GameMode;
        _timerVisibilityToggle.isOn = _config.ShowTimer;
        _timer.SetVisible(_config.ShowTimer);
        _timeLimitField.text = _timer.TimeLimit.ToString();
        _amountField.text = _config.SubmissionLimit.ToString();
        UpdateTimerConfigDisplay();
        UpdateAmountConfigDisplay();
        _config.OnGameModeChanged += _ => 
        {
            UpdateTimerConfigDisplay();
            UpdateAmountConfigDisplay();
        };

        SwitchLibrary(_defaultLibrary);
    }

    #endregion
    

    #region Utils

    public void SetGameMode(int mode)
    {
        var newMode = (GameMode)mode;
        if(newMode == GameMode.Timer)
        {
            _timer.SetMode(Timer.TimerMode.Timer);
        }

        else
        {
            _timer.SetMode(Timer.TimerMode.Chrono);
        }

        _config.GameMode = newMode;
    }

    public void SetTimerLimit(string timeLimit)
    {
        _timer.SetTimerLimit(float.Parse(timeLimit));
    }

    public void SetAmount(string amount)
    {
        _config.SubmissionLimit = int.Parse(amount);
    }

    public void SetAllCharacterToggle(bool value)
    {
        foreach (GridToggle toggle in _characterToggles)
        {
            if(!toggle.Interactable) continue;
            
            toggle.ToggleItem(value);
        }
    }

    public void SwitchLibrary(CharacterLibrary library)
    {
        _currentEditedLibrary = library;
        foreach (var item in _characterToggles)
        {
            if(!item.TryGetComponent<CharacterToggle>(out var toggle)) continue;

            toggle.Library = _currentEditedLibrary;
        }
    }

#if UNITY_EDITOR
    public void FetchCharacterToggles()
    {
        _characterToggles = _characterToggleContainer.GetComponentsInChildren<GridToggle>();
        for (int i = 0; i < _characterToggles.Length; i++)
        {
            var toggle =_characterToggles[i];
            //first row
            if(i <= 15)
            {
                UnityEventTools.AddIntPersistentListener(toggle.Toggle.onValueChanged, ToggleColumn, i);
            }

            else if(i % _rowLength == 0)
            {
                UnityEventTools.AddIntPersistentListener(toggle.Toggle.onValueChanged, ToggleRow, i);
            }

            else
            {
                UnityEventTools.AddIntPersistentListener(toggle.Toggle.onValueChanged, CheckSelectorToggle, i);
            }
        }
    }
#endif
    
    
    private void ToggleRow(int index)
    {
        var toggle = _characterToggles[index].Toggle;
        for (int i = index; i < index + _rowLength; i++)
        {
            _characterToggles[i].ToggleItem(toggle.isOn);
            CheckSelectorToggle(_characterToggles[i].Index);
        }
    }

    private void ToggleColumn(int index)
    {
        var toggle = _characterToggles[index].Toggle;
        for (int i = index; i < _characterToggles.Length; i += _rowLength)
        {
            _characterToggles[i].ToggleItem(toggle.isOn);
        }
    }

    #endregion


    #region Plumbery

    private void UpdateTimerConfigDisplay()
    {
        _timerLimitDisplay.gameObject.SetActive(_config.GameMode == GameMode.Timer);
    }
    
    private void UpdateAmountConfigDisplay()
    {
        _amountDisplay.gameObject.SetActive(_config.GameMode == GameMode.Amount);
    }

    private void CheckSelectorToggle(int index)
    {
        CheckColumnSelector(index);
        CheckRowSelector(index);
    }

    private void CheckRowSelector(int index)
    {
        for (int i = _rowLength; i < _characterToggles.Length; i += _rowLength)
        {
            var isRightRowIndex = index < i + _rowLength;
            if (!isRightRowIndex) continue;

            for (int j = i; j < i + 15; j++)
            {
                //search for any toggled on character
                if (!_characterToggles[j].Toggle.isOn) continue;

                _characterToggles[i].ToggleItem(true);
                return;
            }

            _characterToggles[i].ToggleItem(false);
            return;
        }
    }

    private void CheckColumnSelector(int index)
    {
        for (int i = 1; i < _rowLength; i++)
        {
            var isRightColumnIndex = index % _rowLength == i;
            if (!isRightColumnIndex) continue;

            for (int j = index; j < _characterToggles.Length; j += _rowLength)
            {
                //search for any toggled on character
                if (!_characterToggles[j].Toggle.isOn) continue;

                _characterToggles[i].ToggleItem(true);
                return;
            }

            _characterToggles[i].ToggleItem(false);
            return;
        }
    }


    #endregion


    #region Private Fields

    private const int _rowLength = 16;
    private CharacterLibrary _currentEditedLibrary;
        
    #endregion
}

public enum GameMode
{
    Free,
    Amount,
    Timer,
}