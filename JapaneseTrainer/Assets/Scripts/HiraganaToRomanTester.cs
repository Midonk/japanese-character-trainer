using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Serialization;

public class HiraganaToRomanTester : MonoBehaviour
{
    #region Exposed

    [SerializeField] private GameData _data;
    [SerializeField] private CharacterLibrary[] _libraries;
    [SerializeField] private Timer _timer;

    [Header("Trial")]
    [FormerlySerializedAs("_hiraganaDisplay")]
    [SerializeField] private TMP_Text _characterDisplay;
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
        if(_enabledCharacters.Length == 0)
        {
            //Create a modal to inform player
            Debug.LogError("No character enabled in the character set");
            _onTrialStarted?.Invoke();
            return;
        }

        _data.Reset();
        //_mistakeReport.Clear();

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
        _onTrialEnded?.Invoke();
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
        }
    }

    //use a pool ?
    private void BuildMistakeReport()
    {
        foreach (Transform child in _mistakePanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in _mistakeReport)
        {
            //if(item.Value == 0) continue;

            var mistakeDisplay = Instantiate(_mistakeDisplayPrefab, _mistakePanel);
            var mistakeTexts = mistakeDisplay.GetComponentsInChildren<TMPro.TMP_Text>();
            var nativeCharacter = mistakeTexts[0];
            var mistakeCount = mistakeTexts[1];
            var library = _libraries[item.Key.LibraryIndex];
            nativeCharacter.text = library.Characters[item.Key.CharacterIndex].Native;
            mistakeCount.text = item.Value.ToString();
        }
    }

    private void DrawCharacter()
    {
        var selectedCharacterIndex = Random.Range(0, _enabledCharacters.Length);
        _currentEnabledCharacterIndex = selectedCharacterIndex;
        _characterDisplay.text =  CurrentLibrary.Characters[CurrentCharacterTable.CharacterIndex].Native;
    }

    private void VerifySubmition()
    {
        _data.SubimissionCount ++;
        if(IsAnswerCorrect)
        {
            _data.Score ++;
        }

        else if(!_mistakeReport.ContainsKey(CurrentCharacterTable))
        {
            _mistakeReport.Add(CurrentCharacterTable, 1);
        }
        
        else
        {
            _mistakeReport[CurrentCharacterTable] ++;
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
        var enabledCharacters = new List<CharacterTable>();
        for (int j = 0; j < _libraries.Length; j++)
        {
            for (var i = 0; i < _libraries[j].Characters.Length; i++)
            {
                if (!_libraries[j].Characters[i].Enable) continue;

                enabledCharacters.Add(new CharacterTable
                {
                    LibraryIndex = j,
                    CharacterIndex = i
                });
            }
        }

        _enabledCharacters = enabledCharacters.ToArray();
    }

    private void SetupMistakeReport()
    {
        _mistakeReport = new Dictionary<CharacterTable, int>(_enabledCharacters.Length);
        /* for (int i = 0; i < _enabledCharacters.Length; i++)
        {
            _mistakeReport.Add(_enabledCharacters[i], 0);
        } */
    }

    #endregion


    #region Private Fields

    private int _currentEnabledCharacterIndex;
    private Dictionary<CharacterTable, int> _mistakeReport;
    private CharacterTable[] _enabledCharacters;
    private bool IsAnswerCorrect => !string.IsNullOrWhiteSpace(_userInput.text) && _userInput.text.ToLower().Equals(CurrentLibrary.Characters[CurrentCharacterTable.CharacterIndex].Latin);
    public CharacterLibrary CurrentLibrary => _libraries[CurrentCharacterTable.LibraryIndex];
    private CharacterTable CurrentCharacterTable => _enabledCharacters[_currentEnabledCharacterIndex];

    #endregion


    private struct CharacterTable
    {
        public int LibraryIndex;
        public int CharacterIndex;
    }
}


//あいうえおかきくけこさしすせそたちつてとなにぬねの
//アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤュヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポ