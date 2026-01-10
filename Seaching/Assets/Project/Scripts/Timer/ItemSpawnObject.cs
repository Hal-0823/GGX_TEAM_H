using UnityEngine;

/// <summary>
/// 破壊されたときに、アイテムをスポーンさせるクラス
/// </summary>
public class ItemSpawnObject : MonoBehaviour
{
    [SerializeField] private GameObject ItemPrefab;

    public void SpawnItem()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1.0f; // 少し上にずらす
        Instantiate(ItemPrefab, spawnPosition, Quaternion.Euler(0, 180f, 0));
    }
}