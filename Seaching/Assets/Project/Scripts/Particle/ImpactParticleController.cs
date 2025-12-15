using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ImpactParticleController : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float emissionMultiplier = 5.0f; // 衝撃力 x この倍率 = 出る数
    [SerializeField] private int minParticles = 5;            // 最低でも出る数
    [SerializeField] private int maxParticles = 100;          // 出すぎ防止の上限

    [Header("オプション：サイズも変える？")]
    [SerializeField] private bool scaleSizeWithImpact = true;
    [SerializeField] private float baseSize = 1.0f;

    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 衝撃を与えてパーティクルを発生させる
    /// </summary>
    /// <param name="breakCount">壊した数</param>
    public void PlayEffect(int breakCount)
    {
        // 1. 出す数を計算
        int count = Mathf.FloorToInt(breakCount * emissionMultiplier);
        
        // 範囲内に収める（0個になったり、10000個でフリーズするのを防ぐ）
        count = Mathf.Clamp(count, minParticles, maxParticles);

        // 2. (オプション) 衝撃が強いと粒も大きくする
        if (scaleSizeWithImpact)
        {
            var main = ps.main;
            // 衝撃が強いほど大きくなる（例：弱い=1倍, 強い=2倍...）
            // impactForceの目安が 10 くらいなら 0.1 をかけるなど調整
            main.startSizeMultiplier = baseSize * (1.0f + breakCount * 0.05f);
        }

        // 3. 発射！
        ps.Emit(count);
        
        // デバッグ用（調整時に便利）
        //Debug.Log($"衝撃: {breakCount:F1} -> パーティクル数: {count}");
    }
}