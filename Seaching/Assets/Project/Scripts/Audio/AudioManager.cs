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
    public float MasterVolume { get; set; } = 0.9f;
    public float BgmVolume { get; set; } = 0.35f;
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

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="key"></param>
    public void PlaySE(string key)
    {
        if (seMap.TryGetValue(key, out var entry))
        {
            float finalVol = entry.volume * SeVolume * MasterVolume;
            // ピッチが1.0（標準）なら、いつもの軽い方法で再生
            if (Mathf.Approximately(entry.pitch, 1.0f))
            {
                seSource.pitch = 1.0f;
                seSource.PlayOneShot(entry.clip, finalVol);
            }
            else
            {
                // ピッチが違う場合は、音が混ざらないように「使い捨てAudioSource」を作る
                PlayClipWithVariablePitch(entry.clip, finalVol, entry.pitch);
            }
        }
        else
        {
            Debug.LogWarning($"SE Not Found: {key}");
        }
    }

    /// <summary>
    /// ピッチ指定付きSE再生
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pitch"></param>
    public void PlaySE(string key, float pitch)
    {
        if (seMap.TryGetValue(key, out var entry))
        {
            float finalVol = entry.volume * SeVolume * MasterVolume;
            // ピッチが1.0（標準）なら、いつもの軽い方法で再生
            if (Mathf.Approximately(pitch, 1.0f))
            {
                seSource.pitch = 1.0f;
                seSource.PlayOneShot(entry.clip, finalVol);
            }
            else
            {
                // ピッチが違う場合は、音が混ざらないように「使い捨てAudioSource」を作る
                PlayClipWithVariablePitch(entry.clip, finalVol, pitch);
            }
        }
        else
        {
            Debug.LogWarning($"SE Not Found: {key}");
        }
    }

    // 使い捨てのAudioSourceを作って鳴らすヘルパー関数
    private void PlayClipWithVariablePitch(AudioClip clip, float volume, float pitch)
    {
        // 空のオブジェクトを作る
        GameObject tempObj = new GameObject("TempSE");
        tempObj.transform.position = Camera.main.transform.position; // カメラ位置で鳴らす（2D的に聞こえるように）

        // AudioSourceをつけて設定
        AudioSource tempSource = tempObj.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.pitch = pitch;
        
        // 再生！
        tempSource.Play();

        // 鳴り終わった頃にオブジェクトごと削除する (クリップの長さ / ピッチ = 再生時間)
        Destroy(tempObj, clip.length / pitch + 0.1f);
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
            bgmSource.pitch = entry.pitch;
            bgmSource.Play();
        });

        // 3. フェードイン
        seq.Append(bgmSource.DOFade(targetVolume, fadeDuration));

        // ※シーン遷移などでオブジェクトが消えてもエラーにならないようLinkしておく
        seq.SetLink(gameObject);
    }
    
    /// <summary>
    /// BGMのピッチを変更する（フェード付き）
    /// </summary>
    /// <param name="newPitch"></param>
    /// <param name="fadeDuration"></param>
    public void ChangeBGMPitch(float newPitch, float fadeDuration = 0.5f)
    {
        // DOTweenシーケンスで「フェードアウト → ピッチ変更 → フェードイン」
        Sequence seq = DOTween.Sequence();

        // 1. フェードアウト
        if (bgmSource.isPlaying)
        {
            seq.Append(bgmSource.DOFade(0f, fadeDuration));
        }

        // 2. ピッチ変更
        seq.AppendCallback(() =>
        {
            bgmSource.pitch = newPitch;
        });

        // 3. フェードイン
        float targetVolume = BgmVolume * MasterVolume;
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