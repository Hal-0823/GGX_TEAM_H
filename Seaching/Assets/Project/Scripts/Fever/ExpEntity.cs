using UnityEngine;
using UnityEngine.Pool;
using System;

/// <summary>
/// フィーバーゲージを増加させるEXPエンティティ
/// </summary>
[RequireComponent(typeof(Magnet))]
public class ExpEntity : MonoBehaviour
{
    /// <summary>
    /// EXPが収集されたときのイベント
    /// </summary>
    public static event Action<int> OnExpCollected;

    [SerializeField] private int expValue = 1;

    // 自分が所属しているプール
    private IObjectPool<ExpEntity> pool;

    private void Start()
    {
        Magnet magnet = GetComponent<Magnet>();
        magnet.OnCollected += HandleCollected;  // Magnetクラスの収集イベントに登録
    }

    /// <summary>
    /// 生成時にプール側からこのメソッドを呼び出して、参照をセットする
    /// </summary>
    /// <param name="objectPool"></param>
    public void SetPool(IObjectPool<ExpEntity> objectPool)
    {
        pool = objectPool;
    }

    /// <summary>
    /// プールから再利用されるたびに呼び出される初期化処理
    /// </summary>
    public void ResetState()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 指定位置にスポーンする
    /// </summary>
    /// <param name="position"></param>
    public void SpawnAt(Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// EXPが収集されたときの処理
    /// </summary>
    private void HandleCollected()
    {
        OnExpCollected?.Invoke(expValue);
        Destroy(gameObject);
    }


}