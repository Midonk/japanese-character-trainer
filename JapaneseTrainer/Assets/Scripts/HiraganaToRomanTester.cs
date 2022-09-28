using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HiraganaToRomanTester : MonoBehaviour
{
    #region Exposed

    [SerializeField] private GameData _data;
    [SerializeField] private CharacterLibrary _library;
    [SerializeField] private Timer _timer;

    [Header("Trial")]
    [SerializeField] private TMP_Text _hiraganaDisplay;
    [SerializeField] private TMP_InputField _userInput;

    [Header("Report")]
    [SerializeField] private TMP_Text _scoreDisplay;
    [SerializeField] private TMP_Text _submissionDisplay;
    [SerializeField] private GameObject _mistakeDisplayPrefab;
    [SerializeField] private RectTransform _mistakePanel;
    [SerializeField] private TMP_Text _secondsPerChar;

    [Header("Events")]
    [SerializeField] private UnityEvent _onTrialStarted;
    [SerializeField] private UnityEvent _onTrialEnded;
    [SerializeField] private UnityEvent _onSubmit;
        
    #endregion
    
    
    #region Unity API

    private void Awake() 
    {
        _userInput.onSubmit.AddListener(_ => VerifySubmition());
    }
        
    #endregion

    
    #region Main

    [ContextMenu("Start trial")]
    public void StartTrial()
    {
        SetupTrial();
        _data.Reset();
        for (int i = 0; i < _enabledIndexes.Length; i++)
        {
            _mistakeReport[_enabledIndexes[i]] = 0;
        }

        DrawCharacter();
        _onTrialStarted?.Invoke();
    }

    [ContextMenu("Stop trial")]
    public void StopTrial()
    {
        _scoreDisplay.text = _data.Score.ToString();
        _submissionDisplay.text = _data.SubimissionCount.ToString();
        BuildMistakeReport();
        CalculateTiming();
    }
        
    #endregion

    
    #region Utils

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    #endregion
    

    #region Plumbery

    private void CalculateTiming()
    {
        var averageTiming = _timer.Mode switch
        {
            Timer.TimerMode.Timer => _timer.TimeLimit - _timer.Time,
            _ => _timer.Time
        };

        if (_data.SubimissionCount == 0)
        {
            _secondsPerChar.text = "n/a";
        }

        else
        {
            averageTiming /= _data.SubimissionCount;
            _secondsPerChar.text = $"{averageTiming: #0.00} per character";

            _onTrialEnded?.Invoke();
        }
    }

    private void BuildMistakeReport()
    {
        foreach (Transform child in _mistakePanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in _mistakeReport)
        {
            if(item.Value == 0) continue;

            var mistakeDisplay = Instantiate(_mistakeDisplayPrefab, _mistakePanel);
            var mistakeTexts = mistakeDisplay.GetComponentsInChildren<TMPro.TMP_Text>();
            var hiragana = mistakeTexts[0];
            var mistakeCount = mistakeTexts[1];
            hiragana.text = _library.Characters[item.Key].Hiragana;
            mistakeCount.text = item.Value.ToString();
        }
        /* for (int i = 0; i < _enabledIndexes.Length; i++)
        {
            if (_mistakeReport[i] == 0) continue;

            var mistakeDisplay = Instantiate(_mistakeDisplayPrefab, _mistakePanel);
            var mistakeTexts = mistakeDisplay.GetComponentsInChildren<TMPro.TMP_Text>();
            var hiragana = mistakeTexts[0];
            var mistakeCount = mistakeTexts[1];
            hiragana.text = _library.Characters[_enabledIndexes[i]].Hiragana;
            mistakeCount.text = _mistakeReport[i].ToString();
        } */
    }

    private void DrawCharacter()
    {
        var selectedIndex = Random.Range(0, _enabledIndexes.Length);
        _currentHiraganaIndex = _enabledIndexes[selectedIndex];
        _hiraganaDisplay.text = _library.Characters[_currentHiraganaIndex].Hiragana;
    }

    private void VerifySubmition()
    {
        _data.SubimissionCount ++;
        if(IsAnswerCorrect)
        {
            _data.Score ++;
        }

        else
        {
            _mistakeReport[_currentHiraganaIndex] ++;
        }

        _userInput.text = string.Empty;
        _onSubmit?.Invoke();
        DrawCharacter();
    }

    private void SetupTrial()
    {
        FetchEnabledCharacters();
        SetupMistakeReport();
    }

    private void FetchEnabledCharacters()
    {
        var enabledIndexes = new List<int>();
        for (var i = 0; i < _library.Characters.Length; i++)
        {
            if (!_library.Characters[i].Enable) continue;

            enabledIndexes.Add(i);
        }

        _enabledIndexes = enabledIndexes.ToArray();
    }

    private void SetupMistakeReport()
    {
        _mistakeReport = new Dictionary<int, int>(_enabledIndexes.Length);
        for (int i = 0; i < _enabledIndexes.Length; i++)
        {
            _mistakeReport.Add(_enabledIndexes[i], 0);
        }
    }

    #endregion


    #region Private Fields

    private int _currentHiraganaIndex;
    private Dictionary<int, int> _mistakeReport;
    private int[] _enabledIndexes;

    private bool IsAnswerCorrect => !string.IsNullOrWhiteSpace(_userInput.text) && _userInput.text.Equals(_library.Characters[_currentHiraganaIndex].Latin);

    #endregion
}

//あいうえおかきくけこさしすせそたちつてとなにぬねの