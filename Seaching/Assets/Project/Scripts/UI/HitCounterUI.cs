using UnityEngine;
using TMPro;
using DG.Tweening; // ★必須

[RequireComponent(typeof(CanvasGroup))]
public class HitCounterUI : MonoBehaviour
{
    public static HitCounterUI instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hitText;
    
    [Header("Animation Settings")]
    [SerializeField] private float punchStrength = 1.0f; // 大きくなる倍率（0.5～1.5くらい）
    [SerializeField] private float duration = 0.2f;      // 演出にかかる時間
    [SerializeField] private int vibrato = 10;           // 震える回数（ボヨンボヨン感）
    [SerializeField] private float elasticity = 1.0f;    // 反発係数（大きいとビヨンと伸びる）

    [Header("Logic")]
    [SerializeField] private float comboResetTime = 1.5f; // 表示が消えるまでの待機時間

    // 内部変数
    private int currentHits = 0;
    private float resetTimer = 0f;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    private void Awake()
    {
        if (instance == null) instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (hitText)
        {
            originalScale = hitText.transform.localScale;
            // 最初は透明にしておく
            canvasGroup.alpha = 0f; 
        }
    }

    private void Update()
    {
        if (resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0)
            {
                HideCounter();
            }
        }
    }

    public void AddHit()
    {
        currentHits++;
        resetTimer = comboResetTime;

        if (hitText)
        {
            hitText.text = currentHits + "<size=70%>HITS!</size>";
            
            // ★DOTweenの演出パート
            
            // 1. 前のアニメーションがあれば強制停止してリセット（連打対応）
            hitText.transform.DOKill(); 
            hitText.transform.localScale = originalScale;
            canvasGroup.DOKill();
            canvasGroup.alpha = 1f; // 即座に表示

            // 2. パンチ演出（ボヨンと拡大して戻る）
            // DOPunchScale(強さ, 時間, 振動数, 弾力)
            hitText.transform.DOPunchScale(Vector3.one * punchStrength, duration, vibrato, elasticity)
                .SetEase(Ease.OutQuad); // 勢いよく飛び出るイージング

            // 3. おまけ：文字色を一瞬光らせる（白から元の色へ）
            // ※TextMeshProのvertex colorを使う場合
            hitText.color = Color.white;
            hitText.DOColor(Color.yellow, 0.1f); // 黄色に戻るなど
        }
    }

    public void ForceReset()
    {
        currentHits = 0;
        resetTimer = 0;
        // スッと消す
        canvasGroup.DOFade(0f, 0.2f);
    }

    private void HideCounter()
    {
        currentHits = 0;
        // フワッとフェードアウトさせて消す
        canvasGroup.DOFade(0f, 0.5f);
    }
}