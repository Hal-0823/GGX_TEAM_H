using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 破壊可能なターゲットを表すクラス
/// </summary>
public class BreakableObject : MonoBehaviour
{
    public bool IsBroken { get; private set; } = false;

    // 状態が変化したときに通知するイベント
    [HideInInspector]
    public UnityEvent OnStateChanged;

    [Header("見た目の設定")]
    [SerializeField, Tooltip("通常状態の見た目")]
    private GameObject normalModel;
    [SerializeField, Tooltip("破壊状態の見た目")]
    private GameObject brokenModel;

    [Header("当たり判定")]
    [SerializeField, Tooltip("当たり判定用のコライダー")]
    private Collider targetCollider;

    // 破壊のメソッド
    [ContextMenu("このターゲットを破壊する")]
    public void Break()
    {
        if (IsBroken) return;

        SetBrokenState(true);
    }

    // リセットや修復のためのメソッド
    [ContextMenu("このターゲットを修復する")]
    public void Repair()
    {
        SetBrokenState(false);
    }
    
    private void SetBrokenState(bool isBroken)
    {
        IsBroken = isBroken;

        // モデルの切り替え
        if (normalModel != null) normalModel.SetActive(!isBroken);
        if (brokenModel != null) brokenModel.SetActive(isBroken);

        if (targetCollider != null) targetCollider.enabled = !isBroken;

        // 状態変化の通知
        OnStateChanged?.Invoke();
    }
}