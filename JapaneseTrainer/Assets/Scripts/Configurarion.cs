using UnityEngine;
using UnityEngine.UI;

public class Configurarion : MonoBehaviour
{
    #region Exposed

    [Header("Global")]
    [SerializeField] private GameConfig _config;
    [SerializeField] private CharacterLibrary _library;
    [SerializeField] private Timer _timer;
    [SerializeField] private TMPro.TMP_Dropdown _gameModeDisplay;
    

    [Header("Character enabling")]
    [SerializeField] private RectTransform _characterToggleContainer;
    [SerializeField] private GameObject _characterTogglePrefab;
    
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
        SetupCharacterConfig();
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
        foreach (Toggle toggle in _characterToggles)
        {
            toggle.isOn = value;
        }
    }
        
    #endregion

    
    #region Plumbery

    private void SetupCharacterConfig()
    {
        _characterToggles = new Toggle[_library.Characters.Length];
        for (int i = 0; i < _library.Characters.Length; i++)
        {
            var index = i;
            var toggleDisplay = Instantiate(_characterTogglePrefab, _characterToggleContainer);
            var characterDisplay = toggleDisplay.GetComponentInChildren<TMPro.TMP_Text>();
            var toggle = toggleDisplay.GetComponentInChildren<Toggle>();
            var character = _library.Characters[i];
            characterDisplay.text = $"{character.Hiragana} ({character.Latin})";
            toggle.isOn = character.Enable;
            toggle.onValueChanged.AddListener(OnToggleChanged);
            _characterToggles[i] = toggle;

            void OnToggleChanged(bool value)
            {
                _library.Characters[index].Enable = value;
            }
        }
    }

    private void UpdateTimerConfigDisplay()
    {
        _timerLimitDisplay.gameObject.SetActive(_config.GameMode == GameMode.Timer);
    }
    
    private void UpdateAmountConfigDisplay()
    {
        _amountDisplay.gameObject.SetActive(_config.GameMode == GameMode.Amount);
    }

    #endregion


    #region Private Fields

    private Toggle[] _characterToggles;
        
    #endregion
}

public enum GameMode
{
    Free,
    Amount,
    Timer,
}