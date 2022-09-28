using UnityEngine;

[CreateAssetMenu(menuName = "Game Data", fileName = "new Game Data")]
public class GameData : ScriptableObject
{
    public int Score;
    public int SubimissionCount;

    internal void Reset()
    {
        Score = 0;
        SubimissionCount = 0;
    }
}
