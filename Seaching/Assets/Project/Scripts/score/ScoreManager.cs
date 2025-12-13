using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameSessionData sessionData;

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

    public int CurrentTotalScore => currentTotalScore;

    private void Awake()
    {
        if (Instance == null)//シーンを跨いでも値を破棄しない
        {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        pendingScore = 0;
        currentTotalScore = 0;
        displayedTotalScore = 0;
        currentTotalScoreText.text = "0";
        pendingScoreText.text = "";
        UpdateText();
    }

    private void OnEnable()//score加算
    {
        BreakableObject.OnObjectBroken += AddScore;
        BravePersonController.OnBraveDefeated += AddScore;
        HitCounterUI.OnCounterReset += FinalizeCombo;
    }

        private void OnDisable()//score減少
    {
        BreakableObject.OnObjectBroken -= AddScore;
        BravePersonController.OnBraveDefeated -= AddScore;
        HitCounterUI.OnCounterReset -= FinalizeCombo;
    }

    // トータルスコアに合算する
    private void AddTotalScore(int amount)
    {
        // 1. 目標スコアを更新
        currentTotalScore += amount;
        sessionData.currentScore = currentTotalScore;

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

    // 保留中のスコアを加算する
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

    // 保留中のスコアに倍率をかけて確定させる
    private void MulScore(float multiplier)
    {
        int finalAddScore = Mathf.FloorToInt(pendingScore * multiplier);

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            pendingScoreText.text = $"+{pendingScore} × {multiplier:F1}"; 
        });

        seq.AppendInterval(0.7f);

        seq.Append(pendingScoreText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f));

        seq.InsertCallback(0.2f + 0.7f, () => 
        {
            pendingScoreText.color = Color.yellow;
            pendingScoreText.text = $"+{finalAddScore}";
        });

        seq.AppendInterval(0.5f);

        seq.AppendCallback(() => AddTotalScore(finalAddScore));

        seq.OnComplete(() => 
        {
            pendingScoreText.transform.localScale = Vector3.one;
            pendingScoreText.color = Color.white;
            // UIを消す演出
            pendingScoreCanvasGroup.DOFade(0f, 0.5f);

            // 溜め込み表示をリセット
            pendingScore = 0;
        });

        seq.Play();
    }
    
    // コンボ終了時に保留中のスコアを確定させる
    private void FinalizeCombo(int hitCount)
    {
        float bonusMultiplier = 1.0f + hitCount * 0.1f; // 例: ヒット数に応じて10%ずつボーナス
        if (pendingScore > 0)
        {
            MulScore(bonusMultiplier);
            return;
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

