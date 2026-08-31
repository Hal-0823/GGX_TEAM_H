using UnityEngine;
using System.Collections.Generic;

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

    // 既に固まって背景化した破片の待ち行列（古い順に並ぶ）
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
                    SolidifyAndQueue(data);
                    RemoveAtFast(i);
                }
            }
        }
    }

    // 物理演算を削除して定員チェックを行う
    private void SolidifyAndQueue(DebrisData data)
    {
        // 物理と判定を消す（軽量化）
        if (data.rb != null) Destroy(data.rb);
        if (data.col != null) Destroy(data.col);

        solidifiedDebrisQueue.Enqueue(data.gameObject);

        if (solidifiedDebrisQueue.Count > maxSolidDebrisCount)
        {
            // 一番古い瓦礫を取り出す
            GameObject oldDebris = solidifiedDebrisQueue.Dequeue();
            
            if (oldDebris != null)
            {
                //oldDebris.AddComponent<DebrisShrinker>();
                Destroy(oldDebris);
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