using UnityEngine;
using System;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class HitCounterUI : MonoBehaviour
{
    public static HitCounterUI instance;
    public static Action<int> OnCounterReset;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hitText;
    
    [Header("Rank Settings")]
    [SerializeField] private List<HitColorRank> colorRanks = new List<HitColorRank>();

    [System.Serializable]
    public struct HitColorRank
    {
        public int threshold;        // 閾値
        public float targetFontSize; // ★このランクでのフォントサイズ
        public Color topColor;
        public Color bottomColor;
    }

    [Header("Animation Settings")]
    [SerializeField] private float fontChangeSpeed = 0.3f; // フォントサイズが変化する速さ
    [SerializeField] private float punchScale = 1.2f;      // ヒット時の跳ねる大きさ
    [SerializeField] private float comboResetTime = 2.0f;

    // 内部変数
    private int currentHits = 0;
    private float resetTimer = 0f;
    private CanvasGroup canvasGroup;
    private Tween fontSizeTween; // フォントサイズのTween保存用

    private void Awake()
    {
        if (instance == null) instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (hitText)
        {
            canvasGroup.alpha = 0f;
            hitText.enableVertexGradient = true;
            // 重要：Auto SizeがONだとスクリプトからサイズ変更できないためOFFにする
            hitText.enableAutoSizing = false; 
        }
    }

    private void Update()
    {
        if (resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0) HideCounter();
        }
    }

    public void AddHit()
    {
        currentHits++;
        resetTimer = comboResetTime;

        if (hitText)
        {
            hitText.text = currentHits + "<size=70%> HITS!</size>";
            
            // 表示ON
            canvasGroup.DOKill();
            canvasGroup.alpha = 1f;

            // --- 1. ランク情報の取得 ---
            HitColorRank currentRank = GetCurrentRank(currentHits);

            // --- 2. 色の更新 ---
            hitText.colorGradient = new VertexGradient(
                currentRank.topColor, currentRank.topColor, 
                currentRank.bottomColor, currentRank.bottomColor
            );

            // --- 3. フォントサイズの変更（DOTween.Toを使用） ---
            // いきなり変わるとカクつくので、滑らかに変化させる
            if (hitText.fontSize != currentRank.targetFontSize)
            {
                // 前のサイズ変更アニメーションがあれば止める
                if (fontSizeTween != null && fontSizeTween.IsActive()) fontSizeTween.Kill();

                // 現在のサイズから目標サイズへアニメーション
                // DOTween.To(getter, setter, endValue, duration)
                fontSizeTween = DOTween.To(
                    () => hitText.fontSize, 
                    x => hitText.fontSize = x, 
                    currentRank.targetFontSize, 
                    fontChangeSpeed
                ).SetEase(Ease.OutCubic);
            }

            // --- 4. パンチ演出（跳ねる動きはスケールで行う） ---
            // ※フォントサイズを変えているので、スケールのベースは常に「1」でOK
            hitText.transform.DOKill(); // 重複防止
            hitText.transform.localScale = Vector3.one; // 一旦戻す
            hitText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), 0.2f, 10, 1f);
        }
    }

    private HitColorRank GetCurrentRank(int hits)
    {
        HitColorRank selectedRank = colorRanks[0];
        foreach (var rank in colorRanks)
        {
            if (hits >= rank.threshold) selectedRank = rank;
        }
        return selectedRank;
    }

    public void ForceReset()
    {
        resetTimer = 0;

        HideCounter();
    }

    private void HideCounter()
    {
        canvasGroup.DOFade(0f, 0.5f);
        OnCounterReset?.Invoke(currentHits);
        currentHits = 0;
    }
}