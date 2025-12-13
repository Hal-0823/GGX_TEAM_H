using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // DOTweenを使います

public class DebrisManager : MonoBehaviour
{
    public static DebrisManager Instance;

    [Header("Global Settings")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float maxLifetime = 10.0f;

    [Header("Capacity Settings")]
    [Tooltip("シーンに残しておける瓦礫の最大数")]
    [SerializeField] private int maxSolidDebrisCount = 300; 
    
    // 監視中の動いている破片リスト
    private List<DebrisData> activeDebris = new List<DebrisData>(1000);

    // 既に固まって背景化した破片の「待ち行列」（古い順に並ぶ）
    private Queue<GameObject> solidifiedDebrisQueue = new Queue<GameObject>();

    private struct DebrisData
    {
        public Rigidbody rb;
        public Collider col;
        public float spawnTime;
        public GameObject gameObject;

        public DebrisData(Rigidbody r, Collider c, float time, GameObject go)
        {
            rb = r;
            col = c;
            spawnTime = time;
            gameObject = go;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterDebris(Rigidbody rb, Collider col)
    {
        activeDebris.Add(new DebrisData(rb, col, Time.time, rb.gameObject));
    }

    private void FixedUpdate()
    {
        float currentTime = Time.time;
        
        for (int i = activeDebris.Count - 1; i >= 0; i--)
        {
            DebrisData data = activeDebris[i];

            if (data.rb == null)
            {
                RemoveAtFast(i);
                continue;
            }

            float elapsed = currentTime - data.spawnTime;

            // 停止条件チェック
            if (elapsed >= startDelay)
            {
                if (elapsed >= maxLifetime || data.rb.IsSleeping())
                {
                    SolidifyAndQueue(data); // ★変更：固めて、履歴リストへ
                    RemoveAtFast(i);
                }
            }
        }
    }

    // 物理演算を削除し、定員チェックを行う
    private void SolidifyAndQueue(DebrisData data)
    {
        // 1. 物理と判定を消す（軽量化）
        if (data.rb != null) Destroy(data.rb);
        if (data.col != null) Destroy(data.col);

        // 2. 履歴（Queue）に追加
        solidifiedDebrisQueue.Enqueue(data.gameObject);

        // 3. 定員オーバーしてる？
        if (solidifiedDebrisQueue.Count > maxSolidDebrisCount)
        {
            // 一番古い（最初に入れた）瓦礫を取り出す
            GameObject oldDebris = solidifiedDebrisQueue.Dequeue();
            
            // DOTweenで小さくして消す（パッと消えると不自然なので）
            if (oldDebris != null)
            {
                oldDebris.AddComponent<DebrisShrinker>();
            }
        }
    }

    private void RemoveAtFast(int index)
    {
        int lastIndex = activeDebris.Count - 1;
        if (index < lastIndex)
        {
            activeDebris[index] = activeDebris[lastIndex];
        }
        activeDebris.RemoveAt(lastIndex);
    }
}