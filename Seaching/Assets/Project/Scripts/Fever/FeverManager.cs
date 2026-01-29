using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using DG.Tweening;
using System;

[RequireComponent(typeof(FeverGaugeUI))]
public class FeverManager : MonoBehaviour
{
    public static event Action<bool> OnFeverModeChanged;

    [Header("フィーバーゲージの最大値")]
    [SerializeField] private int maxValue;

    [Header("演出用")]
    [SerializeField] private Volume postProcessingVolume;

    private FeverGaugeUI feverGaugeUI;

    private bool isFever = false;
    private int currentValue = 0;

    // フィーバーモード終了時にイベント登録解除
    private void OnDestroy()
    {
        ExpEntity.OnExpCollected -= IncreaseFeverGauge;
    }

    private void Start()
    {
        feverGaugeUI = GetComponent<FeverGaugeUI>();
        ExpEntity.OnExpCollected += IncreaseFeverGauge;
        feverGaugeUI.UpdateFeverGauge(currentValue, maxValue);

        ChromaticAberration chromaticAberration;
        if (postProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.intensity.value = 0f; // 初期状態では効果を無効化
        }

        Vignette vignette;
        if (postProcessingVolume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f; // 初期状態では効果を無効化
        }

        SetIsFever(false);
    }

    private void IncreaseFeverGauge(int amount)
    {
        if (isFever) return;

        currentValue += amount;
        if (currentValue > maxValue)
        {
            currentValue = maxValue;
        }
        feverGaugeUI.UpdateFeverGauge(currentValue, maxValue);

        if (currentValue >= maxValue)
        {
            TriggerFeverMode();
        }
    }

    private void TriggerFeverMode()
    {
        Debug.Log("フィーバーモード発動！");
        // フィーバーモードの処理をここに追加
        currentValue = 0;
        feverGaugeUI.UpdateFeverGauge(currentValue, maxValue);

        StartCoroutine(FeverModeCoroutine());
    }

    private IEnumerator FeverModeCoroutine()
    {
        SetIsFever(true);
        // フィーバーモード演出開始
        if (postProcessingVolume != null)
        {

            // 例: 色調補正を強調するなど
            // 実際のエフェクト設定はプロジェクトに合わせて調整してください
            ChromaticAberration chromaticAberration;
            if (postProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            {
                chromaticAberration.intensity.value = 1f; // 強調する
            }

            Vignette vignette;
            if (postProcessingVolume.profile.TryGet<Vignette>(out vignette))
            {
                vignette.intensity.value = 0.35f; // 強調する
            }
        }

        AudioManager.Instance.PlaySE("SE_BonusAppear2");
        AudioManager.Instance.ChangeBGMPitch(1.2f, 0.5f); // BGMのピッチを上げる
        feverGaugeUI.PlayFeverStartAnimation(0.5f);
        feverGaugeUI.PlayFeverGaugeDecreaseAnimation(10f);

        // フィーバーモードの持続時間
        yield return new WaitForSeconds(20f);

        // フィーバーモード演出終了
        if (postProcessingVolume != null)
        {
            ChromaticAberration chromaticAberration;
            if (postProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            {
                chromaticAberration.intensity.value = 0f; // 元に戻す
            }

            Vignette vignette;
            if (postProcessingVolume.profile.TryGet<Vignette>(out vignette))
            {
                vignette.intensity.value = 0f; // 元に戻す
            }
        }

        AudioManager.Instance.ChangeBGMPitch(1.0f, 0.2f); // BGMのピッチを元に戻す

        SetIsFever(false);

        Debug.Log("フィーバーモード終了");
    }

    // フィーバーモードの状態を設定するメソッド
    private void SetIsFever(bool value)
    {
        isFever = value;
        OnFeverModeChanged?.Invoke(isFever);
        // フィーバー中ならEXPスポーンを停止する
        ExpSpawner.Instance.SetCanSpawn(!value);
    }
}