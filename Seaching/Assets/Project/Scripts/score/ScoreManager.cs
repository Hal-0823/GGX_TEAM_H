using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentTotalScoreText;
    [SerializeField] private TextMeshProUGUI pendingScoreText;
    [SerializeField] private CanvasGroup pendingScoreCanvasGroup;

    [Header("Animation Settings")]
    [Tooltip("カウントアップにかかる時間（秒）")]
    [SerializeField] private float animationDuration = 0.5f;

    [Tooltip("アニメーションの動き方")]
    [SerializeField] private Ease currentTotalScoreEase = Ease.OutExpo;

    // 保留中のスコア
    private int pendingScore = 0;

    // ゲーム内部の正しいスコア
    private int currentTotalScore = 0;

    // 画面に表示されているスコア
    private int displayedTotalScore = 0;

    private Tween scoreTween;

    private void Awake()
    {
        pendingScore = 0;
        currentTotalScore = 0;
        displayedTotalScore = 0;
        currentTotalScoreText.text = "0";
        pendingScoreText.text = "";
        UpdateText();
    }

    private void OnEnable()
    {
        BreakableObject.OnObjectBroken += AddScore;
        HitCounterUI.OnCounterReset += FinalizeCombo;
    }

        private void OnDisable()
    {
        BreakableObject.OnObjectBroken -= AddScore;
        HitCounterUI.OnCounterReset -= FinalizeCombo;
    }

    private void AddTotalScore(int amount)
    {
        // 1. 目標スコアを更新
        currentTotalScore += amount;

        // 2. 前のアニメーションが途中なら止める（これがないと動きがおかしくなる）
        if (scoreTween != null && scoreTween.IsActive())
        {
            scoreTween.Kill();
        }

        // 3. 表示用の数値を、目標値までアニメーションさせる
        // DOTween.To(getter, setter, endValue, duration)
        scoreTween = DOTween.To(
            () => displayedTotalScore,      // 今の値を渡す
            x =>
            {                          // DOTweenが計算した値をどうするか
                displayedTotalScore = x;    // 変数を更新して...
                UpdateText();               // テキストも書き換える
            },
            currentTotalScore,              // 目標の値
            animationDuration               // かかる時間
        )
        .SetEase(currentTotalScoreEase); // イージング設定

        currentTotalScoreText.transform.DOKill(); // 重複防止
        currentTotalScoreText.transform.localScale = Vector3.one; // リセット
        currentTotalScoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f); // 0.2秒だけ大きく跳ねる
    }

    private void AddScore(int amount)
    {
        pendingScoreCanvasGroup.DOKill();
        pendingScoreCanvasGroup.alpha = 1f;

        pendingScore += amount;
        pendingScoreText.text = "+" + pendingScore.ToString("N0");

        pendingScoreText.transform.DOKill(); // 重複防止
        pendingScoreText.transform.localScale = Vector3.one; // リセット
        pendingScoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f); // 0.2秒だけ大きく跳ねる
    }
    
    // コンボ終了時に保留中のスコアを確定させる
    private void FinalizeCombo(int hitCount)
    {
        if (pendingScore > 0)
        {
            AddTotalScore(pendingScore);
        }

        // UIを消す演出
        pendingScoreCanvasGroup.DOFade(0f, 0.5f);
        
        // 溜め込み表示をリセット
        pendingScore = 0;
    }

    private void UpdateText()
    {
        if (currentTotalScoreText != null)
        {
            currentTotalScoreText.text = displayedTotalScore.ToString("N0"); // 3桁区切りで表示
        }
    }
}

