using UnityEngine;

[RequireComponent(typeof(Magnet))]
public class TimeBonusItem : MonoBehaviour
{
    [SerializeField] private float bonusTime = 10f;
    [SerializeField] private Magnet magnet;

    private void Start()
    {
        magnet.OnCollected += HandleCollected;  // Magnetクラスの収集イベントに登録
        magnet.ChangeIsActive(false);
        Invoke(nameof(EnableMagnet), 1.5f);
    }

    private void HandleCollected()
    {
        GameTimer timer = FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            AudioManager.Instance.PlaySE("SE_ItemGet6");
            timer.AddTime(bonusTime);
        }
        Destroy(gameObject);
    }

    private void EnableMagnet()
    {
        magnet.ChangeIsActive(true);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(EnableMagnet));
    }

    private void OnDestroy()
    {
        magnet.OnCollected -= HandleCollected; // イベント登録解除
    }
}