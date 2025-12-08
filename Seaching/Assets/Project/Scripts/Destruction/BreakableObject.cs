using System;
using UnityEngine;

/// <summary>
/// 破壊可能なターゲットを表すクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BreakableObject : MonoBehaviour
{
    // 破壊状態への変化を通知するイベント
    public static event Action<int> OnObjectBroken;

    [SerializeField, Tooltip("スコア値")]
    private int scoreValue = 100;

    [SerializeField, Tooltip("破壊状態のモデル")]
    private GameObject brokenModel;

    // 破壊処理を行うメソッド
    public void Shatter(Vector3 explosionCenter, float power, float radius)
    {
        Debug.Log($"BreakableObject.Shatter called on {gameObject.name}");
        // 壊れたモデル（破片）を生成
        GameObject brokenObj = Instantiate(brokenModel, transform.position, transform.rotation);
        brokenObj.transform.localScale = transform.localScale; // サイズ合わせ

        // 全ての破片を取得
        Rigidbody[] rbs = brokenObj.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            // 破片に爆発力を加える
            // AddExplosionForceは「中心からの距離」に応じて自動的に威力を減衰させてくれます
            // upwardModifier（第4引数）を少し入れると、破片が地面を擦らずに少し浮き上がるので派手になります
            rb.AddExplosionForce(power, explosionCenter, radius, 3.0f);

            // 不完全な崩壊を防ぐため、建物の中心から、全パーツを少しだけ外に押し出す
            float internalForce = 100f; // 最低限崩すための力
            rb.AddExplosionForce(internalForce, transform.position, 5.0f, 1.0f);
        }

        // 状態変化イベントを発火
        OnObjectBroken?.Invoke(scoreValue);
        Destroy(gameObject); // 元のオブジェクトを削除
    }
}