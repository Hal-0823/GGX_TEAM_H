using UnityEngine;

public class StompAttack : MonoBehaviour
{
    [SerializeField] private float impactRadius = 10f; // 破壊半径
    [SerializeField] private float impactPower = 1000f; // 吹き飛ばす力
    [SerializeField] private LayerMask destructibleLayer; // 建物のレイヤー
    [SerializeField] private ParticleSystem stompEffectPrefab; // 着地エフェクト
    [SerializeField] private Transform stompEffectSpawnPoint; // エフェクトの生成位置

    // アニメーションのイベントや、着地判定から呼び出す
    public void DoStomp()
    {
        // 指定範囲内のコライダを全て取得（SphereColliderを作る代わり）
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, destructibleLayer);

        foreach (Collider hit in hitColliders)
        {
            // 相手が「壊せるもの」か確認
            var building = hit.GetComponent<BreakableObject>();
            if (building != null)
            {
                // 破壊命令を出す（爆発の中心と威力を渡す）
                building.Shatter(transform.position, impactPower, impactRadius);
            }
        }
        
        // ここにカメラシェイクや土煙エフェクトの処理を追加
        if (stompEffectPrefab != null)
        {
            var stompEffect = Instantiate(stompEffectPrefab, stompEffectSpawnPoint.position, Quaternion.identity);
            stompEffect.Play();
        }
    }

    // デバッグ用：エディタ上で範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}