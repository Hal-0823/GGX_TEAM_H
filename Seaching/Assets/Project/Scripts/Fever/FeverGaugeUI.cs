using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FeverGaugeUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Slider feverGaugeSlider;

    public void UpdateFeverGauge(int currentValue, int maxValue)
    {
        if (feverGaugeSlider != null)
        {
            feverGaugeSlider.maxValue = maxValue;
            feverGaugeSlider.value = currentValue;
        }
    }

    public void PlayFeverStartAnimation(float duration)
    {
        if (feverGaugeSlider != null)
        {
            feverGaugeSlider.transform.DOScale(1.2f, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutCubic);
        }
    }

    public void PlayFeverGaugeDecreaseAnimation(float duration)
    {
        if (feverGaugeSlider != null)
        {
            feverGaugeSlider.DOValue(0, duration).SetEase(Ease.OutCubic);
        }
    }
}