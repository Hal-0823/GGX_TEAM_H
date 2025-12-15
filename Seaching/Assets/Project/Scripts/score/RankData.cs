using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RankData", menuName = "Game/Rank Data")]
public class RankData : ScriptableObject
{
    [System.Serializable]
    public class RankEntry
    {
        public string rankName;   // "C", "B", "A", "S", "SSS"...
        public int threshold;     // このスコアを超えたらランクアップ (例: 100000)
        public Color rankColor = Color.white; // ランクの文字色
    }

    // スコアが低い順（C -> B -> A...）に登録することを想定
    public List<RankEntry> ranks = new List<RankEntry>();

    /// <summary>
    /// 現在のスコアに応じたランクを取得する
    /// </summary>
    public RankEntry GetRankByScore(int score)
    {
        // 後ろから見ていき、閾値を超えている最初のランクを返す
        for (int i = ranks.Count - 1; i >= 0; i--)
        {
            if (score >= ranks[i].threshold)
            {
                return ranks[i];
            }
        }
        return ranks[0]; // 最低ランク
    }
}