using UnityEngine;

[RequireComponent(typeof(FeverGaugeUI))]
public class FeverManager : MonoBehaviour
{
    [Header("フィーバーゲージの最大値")]
    [SerializeField] private int maxValue;

    private FeverGaugeUI feverGaugeUI;

    private int currentValue = 0;

    private void Start()
    {
        feverGaugeUI = GetComponent<FeverGaugeUI>();
        ExpEntity.OnExpCollected += IncreaseFeverGauge;
        feverGaugeUI.UpdateFeverGauge(currentValue, maxValue);
    }

    private void IncreaseFeverGauge(int amount)
    {
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
    }
}