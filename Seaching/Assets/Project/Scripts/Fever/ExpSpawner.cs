using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// EXPエンティティのスポーナーおよびオブジェクトプール管理クラス
/// </summary>
public class ExpSpawner : MonoBehaviour
{
    public static ExpSpawner Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private ExpEntity expPrefab;
    [SerializeField] private int defaultCapacity = 30;
    [SerializeField] private int maxCapacity = 100;

    // オブジェクトプール
    private IObjectPool<ExpEntity> expPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        // オブジェクトプールの初期化
        expPool = new ObjectPool<ExpEntity>(
            createFunc: CreateExpEntity,
            actionOnGet: OnGetExpEntity,
            actionOnRelease: OnReleaseExpEntity,
            actionOnDestroy: OnDestroyExpEntity,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxCapacity
        );
    }

    // 新規作成時の処理
    private ExpEntity CreateExpEntity()
    {
        ExpEntity exp = Instantiate(expPrefab);
        exp.SetPool(expPool);
        return exp;
    }

    // プールから取得したときの処理
    private void OnGetExpEntity(ExpEntity exp)
    {
        exp.gameObject.SetActive(true);
        exp.ResetState();
    }

    // プールに返却したときの処理
    private void OnReleaseExpEntity(ExpEntity exp)
    {
        exp.gameObject.SetActive(false);
    }

    // 破棄時の処理
    private void OnDestroyExpEntity(ExpEntity exp)
    {
        Destroy(exp.gameObject);
    }

    // EXPエンティティを指定位置にスポーンする
    public void SpawnExpEntity(Vector3 position)
    {
        var exp = expPool.Get();
        exp.SpawnAt(position);
    }
}