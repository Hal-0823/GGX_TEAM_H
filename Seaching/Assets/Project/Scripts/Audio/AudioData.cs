using System.Collections.Generic;
using UnityEngine;
using System.Linq; // リスト操作に便利

#if UNITY_EDITOR
using UnityEditor; // エディタ拡張機能を使うために必要
#endif

[CreateAssetMenu(fileName = "AudioData", menuName = "Game/Audio Data")]
public class AudioData : ScriptableObject
{
    // 取り込みたいフォルダのパス（Assetsからのパス）
    [Header("Auto Load Settings")]
    [SerializeField] private string bgmPath = "Assets/Audio/BGM";
    [SerializeField] private string sePath = "Assets/Audio/SE";

    [Header("Data List")]
    public List<AudioEntry> bgmList = new List<AudioEntry>();
    public List<AudioEntry> seList = new List<AudioEntry>();

    [System.Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1.0f;
    }

    // =================================================================
    // エディタ拡張機能：インスペクターの歯車アイコンから実行できる機能
    // =================================================================
#if UNITY_EDITOR

    [ContextMenu("Load BGM & SE")]
    private void LoadAllAudio()
    {
        LoadClips(bgmPath, bgmList);
        LoadClips(sePath, seList);
        Debug.Log("音源の取り込みが完了しました！");
    }

    private void LoadClips(string path, List<AudioEntry> targetList)
    {
        // 1. 指定フォルダ内の全オーディオファイルを取得
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { path });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            // 2. すでにリストにあるかチェック（重複登録を防ぐ）
            // ファイル名が同じなら「登録済み」とみなして、Volume設定などは維持する
            var existingEntry = targetList.FirstOrDefault(x => x.clip == clip);

            if (existingEntry == null)
            {
                // 新規登録
                targetList.Add(new AudioEntry
                {
                    key = clip.name, // ファイル名をそのまま呼び出しキーにする
                    clip = clip,
                    volume = 1.0f
                });
            }
            else
            {
                // 名前が変わっているかもしれないので更新（Volumeはいじらない）
                existingEntry.key = clip.name;
            }
        }

        // 3. フォルダから消されたファイルはリストからも消す（掃除）
        // 「現在のリスト」の中で、「見つけたクリップ群」に含まれないものを削除
        // ※ 削除したくない場合はこのブロックを消してください
        for (int i = targetList.Count - 1; i >= 0; i--)
        {
            bool existsInFolder = guids.Any(g => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(g)) == targetList[i].clip);
            if (!existsInFolder)
            {
                targetList.RemoveAt(i);
            }
        }
    }
#endif
}