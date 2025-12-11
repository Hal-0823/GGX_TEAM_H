using UnityEngine;
using System.Collections;

public class BreakAttack : MonoBehaviour
{
    [SerializeField] private float firstImpactRadius = 5f; // 最初の衝撃波半径
    [SerializeField] private float secondImpactRadius = 10f; // 破壊半径
    [SerializeField] private float thirdImpactRadius = 20f; // 外側の衝撃波半径 
    [SerializeField] private float impactPower = 1000f; // 吹き飛ばす力
    [SerializeField] private LayerMask destructibleLayer; // 建物のレイヤー
    [SerializeField] private ParticleSystem stompEffectPrefab; // 着地エフェクト
    [SerializeField] private Transform stompEffectSpawnPoint; // エフェクトの生成位置

    private bool isStopping = false;

    // アニメーションのイベントや、着地判定から呼び出す
    public void DoBreak(float impactRadius, bool isPlayEffect)
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
                if (HitCounterUI.instance != null)
                {
                    HitCounterUI.instance.AddHit();
                }
                if (!isStopping)
                {
                    // ヒットストップを開始
                    StartCoroutine(DoHitStop(0.1f)); // 0.6秒間ヒットストップ
                }
                building.Shatter(transform.position, impactPower, impactRadius);
            }
        }

        // ここにカメラシェイクや土煙エフェクトの処理を追加
        if (isPlayEffect && stompEffectPrefab != null)
        {
            var stompEffect = Instantiate(stompEffectPrefab, stompEffectSpawnPoint.position, Quaternion.identity);
            stompEffect.Play();
        }
    }

    public IEnumerator DoStompCoroutine(int level)
    {

        DoBreak(firstImpactRadius, true);
        if (level < 2)  yield break;
        yield return new WaitForSeconds(0.2f); // 少し待ってから実行
        DoBreak(secondImpactRadius, true);
        if (level < 3)  yield break;
        yield return new WaitForSeconds(0.2f); // 少し待ってから
        DoBreak(thirdImpactRadius, true);
    }

    private IEnumerator DoHitStop(float duration)
    {
        isStopping = true;

        // 1. 時間を止める
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.1f;

        // 2. 指定時間待つ
        // 重要: 普通のWaitForSecondsだと止まった時間のまま永久に待ってしまうため、
        // Realtime（現実時間）を使って計測する
        yield return new WaitForSecondsRealtime(duration);

        // 3. 時間を元に戻す
        Time.timeScale = originalTimeScale;
        
        isStopping = false;
    }

    // デバッグ用：エディタ上で範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firstImpactRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, secondImpactRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, thirdImpactRadius);
    }
}