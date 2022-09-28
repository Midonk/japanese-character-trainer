using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Config", fileName = "new Game Config")]
public class GameConfig : ScriptableObject
{
    public GameMode GameMode
    {
        get => _gameMode;
        set
        {
            _gameMode = value;
            OnGameModeChanged?.Invoke(_gameMode);
        }
    }

    public float TimeLimit;
    public int SubmissionLimit;
    public bool ShowTimer;

    public event Action<GameMode> OnGameModeChanged;
    
    private GameMode _gameMode;
}
