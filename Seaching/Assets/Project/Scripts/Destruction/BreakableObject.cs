using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 破壊可能なターゲットを表すクラス
/// </summary>
public class BreakableObject : MonoBehaviour
{
    // 状態が変化したときに通知するイベント
    [HideInInspector]
    public UnityEvent OnStateChanged;

    [Header("見た目の設定")]
    [SerializeField, Tooltip("通常状態のモデル")]
    private GameObject normalModel;
    [SerializeField, Tooltip("破壊状態のモデル")]
    private GameObject brokenModel;

    // 破壊処理を行うメソッド
    public void Shatter(Vector3 explosionCenter, float power, float radius)
    {
        Debug.Log($"BreakableObject.Shatter called on {gameObject.name}");
        brokenModel.SetActive(true);
        normalModel.SetActive(false);

        // 全ての破片を取得
        Rigidbody[] rbs = brokenModel.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            // 破片に爆発力を加える
            // AddExplosionForceは「中心からの距離」に応じて自動的に威力を減衰させてくれます
            // upwardModifier（第4引数）を少し入れると、破片が地面を擦らずに少し浮き上がるので派手になります
            rb.AddExplosionForce(power, explosionCenter, radius, 3.0f);
        }

        // 状態変化イベントを発火
        OnStateChanged.Invoke();
    }
}