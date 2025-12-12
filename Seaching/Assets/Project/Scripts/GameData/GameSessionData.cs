using UnityEngine;

/// <summary>
/// ゲーム内のデータを、シーンを跨いで保持するためのScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "GameSessionData", menuName = "Game Data/Game Session Data")]
public class GameSessionData : ScriptableObject
{
    public int currentScore;

    public void ResetScore()
    {
        currentScore = 0;
    }
}