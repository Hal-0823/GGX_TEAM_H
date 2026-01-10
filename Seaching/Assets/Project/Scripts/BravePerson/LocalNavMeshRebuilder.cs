using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
public class LocalNavMeshRebuilder : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] NavMeshSurface surface;

    bool isRebuilding;

    void Awake()
    {
        player.OnLanded += RebuildAroundPlayer;
    }

    void OnDestroy()
    {
        // 念のため解除
        player.OnLanded -= RebuildAroundPlayer;
    }

    public void RebuildAroundPlayer()
    {
        if (isRebuilding) return;
        StartCoroutine(RebuildCoroutine());
    }

    IEnumerator RebuildCoroutine()
    {
        isRebuilding = true;

        // プレイヤー中心へ移動
        transform.position = player.transform.position;

        // 破壊・物理反映待ち
        yield return null;
        yield return null;

        surface.BuildNavMesh();
        isRebuilding = false;
    }
}
