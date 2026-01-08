using UnityEngine;
using UnityEngine.UI;

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
}