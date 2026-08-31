using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreDisplay : MonoBehaviour
{
    public event Action OnRankUp;
    public bool IsCompleted => isCompleted;
    private bool isCompleted;

    [SerializeField] private GameSessionData sessionData;
    [SerializeField] private RankData rankData;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI rankText;

    [Header("Count Up Settings")]
    [SerializeField] private float countUpSpeed = 5f;
    [SerializeField] private float soundInterval = 0.08f;
    [SerializeField] private int aimScore = 300000;
    [SerializeField] private float pitchRandomRange = 0.1f;

    private bool isStarted = false;
    private int nowScore = 0;
    private int updateScore = 0;
    private string currentRank = "";
    private float lastSoundTime = 0f; // 最後に鳴らした時間を記録
    private int nextRankIndex;

    void Awake()
    {
        isStarted = false;
        nowScore = 0;
        scoreText.text = "Score:000000";
        rankText.color = rankData.ranks[0].rankColor;
        rankText.text = rankData.ranks[0].rankName;
        nextRankIndex = 1;
        updateScore = sessionData.currentScore;
    }

    void Update()
    {
        if (!isStarted || nowScore >= updateScore) return;
        rankdisplay();
    }

    public void ShowResult()//リザルト画面上に表示する
    {
        isStarted = true;
        //指定した値までカウントアップ
        DOTween.To(
            () => nowScore,
            (n) =>
            {
                nowScore = n;
                scoreText.text = "Score:" + nowScore.ToString("D6");
                TryPlayTickSound(n, aimScore);
            },
            updateScore,
            Mathf.Clamp(updateScore * 0.0001f / countUpSpeed, 2f, 10f))
            .OnComplete(() =>
            {
                isCompleted = true;
            });
    }

    // ランク確定時に呼び出す
    public void ConfirmRank()
    {
        AudioManager.Instance.PlaySE("SE_RankDisplay");
        rankText.transform.DOPunchScale(Vector3.one * 1.2f, 1.0f);
    }

    private void rankdisplay()
    {
        if (nextRankIndex >= rankData.ranks.Count) return;

        var nextRank = rankData.ranks[nextRankIndex];

        if (nowScore >= nextRank.threshold)
        {
            OnRankUp?.Invoke();

            rankText.color = nextRank.rankColor;
            rankText.text = nextRank.rankName;

            //大きさを変えるアニメーション
            rankText.transform.DOScale(1.3f, 0.1f).From(1f).OnComplete(() =>
            {
                rankText.transform.DOScale(1f, 0.1f);
            });

            nextRankIndex++;
        }
    }

    private void TryPlayTickSound(int current, int target)
    {
        // 前回の音から「指定した間隔」以上経過していたら鳴らす
        if (Time.unscaledTime - lastSoundTime >= soundInterval)
        {
            float pitch = 1.0f;
            float progress = (float)current / target; // 0.0 ~ 1.0
            pitch = 0.8f + (progress * 0.3f); // 0.8 ~ 1.3倍まで上がる

            // // ピッチをランダムに変化させる
            // pitch += UnityEngine.Random.Range(-pitchRandomRange, pitchRandomRange);

            AudioManager.Instance.PlaySE("SE_Tick1", pitch); 

            lastSoundTime = Time.unscaledTime;
        }
    }
}
