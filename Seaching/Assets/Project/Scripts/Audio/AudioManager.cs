using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween必須

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private AudioData audioData; // 作ったSOをセット

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    // 高速検索用の辞書
    private Dictionary<string, AudioData.AudioEntry> bgmMap = new Dictionary<string, AudioData.AudioEntry>();
    private Dictionary<string, AudioData.AudioEntry> seMap = new Dictionary<string, AudioData.AudioEntry>();

    // 全体の音量（設定画面などで変更することを想定）
    public float MasterVolume { get; set; } = 1.0f;
    public float BgmVolume { get; set; } = 1.0f;
    public float SeVolume { get; set; } = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // 辞書生成（検索を高速化）
        foreach (var entry in audioData.bgmList) bgmMap[entry.key] = entry;
        foreach (var entry in audioData.seList) seMap[entry.key] = entry;
    }

    // ==========================================================
    // SE再生（ワンショット）
    // ==========================================================
    public void PlaySE(string key)
    {
        if (seMap.TryGetValue(key, out var entry))
        {
            float finalVol = entry.volume * SeVolume * MasterVolume;
            seSource.PlayOneShot(entry.clip, finalVol);
        }
        else
        {
            Debug.LogWarning($"SE Not Found: {key}");
        }
    }

    // ==========================================================
    // BGM再生（クロスフェード付き）
    // ==========================================================
    public void PlayBGM(string key, float fadeDuration = 0.5f)
    {
        if (!bgmMap.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"BGM Not Found: {key}");
            return;
        }

        // 既に同じ曲が流れていたら何もしない
        if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

        // 音量を計算
        float targetVolume = entry.volume * BgmVolume * MasterVolume;

        // DOTweenシーケンスで「フェードアウト → 曲変更 → フェードイン」
        Sequence seq = DOTween.Sequence();

        // 1. 今流れているならフェードアウト
        if (bgmSource.isPlaying)
        {
            seq.Append(bgmSource.DOFade(0f, fadeDuration));
        }

        // 2. 曲を入れ替えて再生開始
        seq.AppendCallback(() => 
        {
            bgmSource.clip = entry.clip;
            bgmSource.Play();
        });

        // 3. フェードイン
        seq.Append(bgmSource.DOFade(targetVolume, fadeDuration));
        
        // ※シーン遷移などでオブジェクトが消えてもエラーにならないようLinkしておく
        seq.SetLink(gameObject); 
    }

    // BGM停止
    public void StopBGM(float fadeDuration = 1.0f)
    {
        bgmSource.DOFade(0f, fadeDuration).OnComplete(() => 
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        });
    }
}