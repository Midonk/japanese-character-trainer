using UnityEngine;
using UnityEngine.Events;

public class SubmissionLimitGameMode : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private GameData _data;
    [SerializeField] private UnityEvent _onGameStop;

    public void CheckEndGame()
    {
        if(_config.GameMode != GameMode.Amount) return;
        if(_data.SubimissionCount < _config.SubmissionLimit) return;

        _onGameStop?.Invoke();
    }
}
