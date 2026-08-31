using System;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// 破壊可能なターゲットを表すクラス
/// </summary>
[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider))]
public class BreakableObject : MonoBehaviour
{
    // 破壊状態への変化を通知するイベント
    public static event Action<int> OnObjectBroken;

    [SerializeField, Tooltip("スコア値")]
    private int scoreValue = 100;

    [SerializeField, Tooltip("破壊状態のモデル")]
    private GameObject brokenModel;

    [SerializeField, Tooltip("破壊時に生成される経験値数")]
    private int expCount = 3;
    
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
            rb.AddExplosionForce(power, explosionCenter, radius, 3.0f);

            // 不完全な崩壊を防ぐため、建物の中心から、全パーツを少しだけ外に押し出す
            float internalForce = 100f; // 最低限崩すための力
            rb.AddExplosionForce(internalForce, transform.position, 5.0f, 1.0f);

            // 破片管理マネージャーに登録して、一定時間後に固めてもらう
            DebrisManager.Instance.RegisterDebris(rb, rb.GetComponent<Collider>());
        }

        // 状態変化イベントを発火
        OnObjectBroken?.Invoke(scoreValue);

        // EXPを生成
        for (int i = 0; i < expCount; i++)
        {
            // ExpSpawnerが存在しない場合は生成をスキップ
            if (ExpSpawner.Instance == null) break;

            // 少しランダムな位置にスポーンさせる（Y軸は元のオブジェクトと同じ高さにする）
            Vector3 spawnPos = transform.position + UnityEngine.Random.insideUnitSphere * 0.8f;
            var exp = ExpSpawner.Instance.SpawnExpEntity(new Vector3(spawnPos.x, transform.position.y + 1.0f, spawnPos.z));

            if (exp.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.WakeUp();
                rb.AddForce(Vector3.up * UnityEngine.Random.Range(5f, 8f), ForceMode.Impulse);
            }

        }

        // アイテム生成
        if (TryGetComponent<ItemSpawnObject>(out ItemSpawnObject itemSpawner))
        {
            itemSpawner.SpawnItem();
        }

        Destroy(gameObject); // 元のオブジェクトを削除
    }
}