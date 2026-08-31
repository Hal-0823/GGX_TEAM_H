using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events; // タイムアップ時のイベント用

public class GameTimer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bonusText;

    [Header("Settings")]
    [SerializeField] private float timeLimit = 60.0f; // 制限時間
    [SerializeField] private float urgencyThreshold = 10.0f; // 赤くなる残り時間

    [Header("Animation")]
    [SerializeField] private Vector3 punchScaleNormal = new Vector3(0.3f, 0.3f, 0f);
    [SerializeField] private Vector3 punchScaleUrgent = new Vector3(0.6f, 0.6f, 0f);
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color urgentColor = Color.red;

    [Header("Events")]
    public UnityEvent OnTimeUp; // タイムアップ時に実行したい処理

    // 内部変数
    private float currentTime;
    private int previousDisplayTime; // 前フレームでの表示秒数
    private bool isRunning = false;

    private void Start()
    {
        // 初期化
        bonusText.text = "";
        currentTime = timeLimit;
        previousDisplayTime = Mathf.CeilToInt(timeLimit);
        
        if(timerText)
        {
            timerText.color = normalColor;
            timerText.text = previousDisplayTime.ToString();
        }
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    private void Update()
    {
        if (!isRunning) return;

        // 時間を減らす
        currentTime -= Time.deltaTime;

        // 0秒以下になったら終了
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            UpdateDisplay(0);
            isRunning = false;
            OnTimeUp?.Invoke(); // ゲームオーバー処理などを呼ぶ
            return;
        }

        int currentDisplayTime = Mathf.CeilToInt(currentTime);

        if (currentDisplayTime != previousDisplayTime)
        {
            UpdateDisplay(currentDisplayTime);
            previousDisplayTime = currentDisplayTime;
        }
    }

    private void UpdateDisplay(int timeToShow)
    {
        if (timerText == null) return;

        timerText.text = timeToShow.ToString();

        // 残り時間が少ない判定
        bool isUrgent = currentTime <= urgencyThreshold;

        timerText.color = isUrgent ? urgentColor : normalColor;

        Vector3 punchPower = isUrgent ? punchScaleUrgent : punchScaleNormal;
        float duration = 0.3f;
        timerText.transform.DOKill();
        timerText.transform.localScale = Vector3.one;
        timerText.transform.DOPunchScale(punchPower, duration, 10, 1);
    }

    // 時間を延長するメソッド（敵を倒したボーナスなどで使う）
    public void AddTime(float amount)
    {
        bonusText.alpha = 1f;
        
        bonusText.text = $"+{amount:F0}";
        Sequence bonusSeq = DOTween.Sequence();
        bonusSeq.Append(bonusText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f));
        bonusSeq.AppendInterval(0.5f);
        bonusSeq.Append(bonusText.DOFade(0f, 0.5f));
        bonusSeq.OnComplete(() =>
        {
            bonusText.text = "";
        });

        currentTime += amount;
        timerText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);
    }
}