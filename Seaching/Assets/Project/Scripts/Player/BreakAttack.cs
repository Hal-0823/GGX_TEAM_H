using System.Collections;
using UnityEngine;

public class BreakAttack : MonoBehaviour
{
    [SerializeField] private float baseFirstImpactRadius = 6f; // 最初の衝撃波半径
    [SerializeField] private float baseSecondImpactRadius = 9f; // 破壊半径
    [SerializeField] private float baseThirdImpactRadius = 13f; // 外側の衝撃波半径 
    [SerializeField] private float impactPower = 1000f; // 吹き飛ばす力
    [SerializeField] private LayerMask destructibleLayer; // 建物と勇者のレイヤー
    [SerializeField] private ParticleSystem stompEffectPrefab; // 着地エフェクト
    [SerializeField] private Transform stompEffectSpawnPoint; // エフェクトの生成位置

    [Header("フィーバー中の強化倍率")]
    [SerializeField] private float feverImpactRadiusMultiplier = 1.5f;

    private bool isFever = false;
    private float firstImpactRadius => isFever ? baseFirstImpactRadius * feverImpactRadiusMultiplier : baseFirstImpactRadius;
    private float secondImpactRadius => isFever ? baseSecondImpactRadius * feverImpactRadiusMultiplier : baseSecondImpactRadius;
    private float thirdImpactRadius => isFever ? baseThirdImpactRadius * feverImpactRadiusMultiplier : baseThirdImpactRadius;

    private bool isStopping = false;

    private void Start()
    {
        FeverManager.OnFeverModeChanged += (feverState) =>
        {
            isFever = feverState;
        };
    }

    // アニメーションのイベントや、着地判定から呼び出す
    public void DoBreak(float impactRadius, bool isPlayEffect)
    {
        int breakCount = 0;
        
        // 指定範囲内のコライダを全て取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, destructibleLayer);

        foreach (Collider hit in hitColliders)
        {
            var building = hit.GetComponent<BreakableObject>();
            if (building != null)
            {
                if (HitCounterUI.instance != null)
                {
                    HitCounterUI.instance.AddHit();
                }
                if (!isStopping)
                {
                    // ヒットストップを開始
                    StartCoroutine(DoHitStop(0.1f));
                }
                building.Shatter(transform.position, impactPower, impactRadius);
                breakCount++;
                continue;
            }

            // フィーバー中は勇者を倒せる
            if (isFever)
            {
                var brave = hit.GetComponent<BravePersonController>();
                if (brave != null)
                {
                    if (HitCounterUI.instance != null)
                    {
                        HitCounterUI.instance.AddHit();
                    }
                    if (!isStopping)
                    {
                        // ヒットストップを開始
                        StartCoroutine(DoHitStop(0.1f));
                    }
                    brave.GetDamage(transform.position, impactPower, impactRadius);
                    AudioManager.Instance.PlaySE("SE_HeroDeath4");
                    
                    continue;
                }
            }
        }

        AudioManager.Instance.PlaySE("SE_Destroy1");

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

        yield return new WaitForSeconds(0.2f);
        DoBreak(secondImpactRadius, true);

        if (level < 3)  yield break;

        yield return new WaitForSeconds(0.2f);
        DoBreak(thirdImpactRadius, true);
    }

    private IEnumerator DoHitStop(float duration)
    {
        isStopping = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(duration);

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