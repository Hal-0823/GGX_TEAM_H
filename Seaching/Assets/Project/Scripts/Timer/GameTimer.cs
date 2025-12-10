using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events; // タイムアップ時のイベント用

public class GameTimer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

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
        currentTime = timeLimit;
        previousDisplayTime = Mathf.CeilToInt(timeLimit);
        
        if(timerText)
        {
            timerText.color = normalColor;
            timerText.text = previousDisplayTime.ToString();
        }
        
        StartTimer();
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

        // 現在の「表示用の整数時間」を計算（切り上げ）
        // 例: 59.9秒 -> 60, 0.1秒 -> 1, 0.0秒 -> 0
        int currentDisplayTime = Mathf.CeilToInt(currentTime);

        // ★ここがポイント：秒数の整数値が変わった瞬間だけ更新＆アニメーション
        if (currentDisplayTime != previousDisplayTime)
        {
            UpdateDisplay(currentDisplayTime);
            previousDisplayTime = currentDisplayTime;
        }
    }

    private void UpdateDisplay(int timeToShow)
    {
        if (timerText == null) return;

        // テキスト更新
        timerText.text = timeToShow.ToString();

        // ピンチ（残り時間が少ない）判定
        bool isUrgent = currentTime <= urgencyThreshold;

        // 色の変更
        timerText.color = isUrgent ? urgentColor : normalColor;

        // アニメーション設定
        Vector3 punchPower = isUrgent ? punchScaleUrgent : punchScaleNormal;
        float duration = 0.3f;

        // 既存のアニメーションをリセットして跳ねさせる
        timerText.transform.DOKill();
        timerText.transform.localScale = Vector3.one; // サイズを戻す
        timerText.transform.DOPunchScale(punchPower, duration, 10, 1);

        // 残り時間が少ないときは、さらに音を鳴らすなどをここに追加すると良い
        // if (isUrgent) SoundManager.Play("TickTock");
    }

    // 時間を延長するメソッド（敵を倒したボーナスなどで使う）
    public void AddTime(float amount)
    {
        currentTime += amount;
        // 上限を超えないようにするならここでClamp
        // currentTime = Mathf.Min(currentTime, timeLimit);
        
        // 時間が増えた演出（緑色に光らせるなど）を入れると親切
        timerText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);
    }
}