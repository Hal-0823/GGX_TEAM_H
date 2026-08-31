using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ImpactParticleController : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float emissionMultiplier = 5.0f;
    [SerializeField] private int minParticles = 5;
    [SerializeField] private int maxParticles = 100;

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
        int count = Mathf.FloorToInt(breakCount * emissionMultiplier);

        count = Mathf.Clamp(count, minParticles, maxParticles);
        if (scaleSizeWithImpact)
        {
            var main = ps.main;
            main.startSizeMultiplier = baseSize * (1.0f + breakCount * 0.05f);
        }

        ps.Emit(count);
    }
}