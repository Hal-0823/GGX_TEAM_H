using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

public class NavMeshRebuildManager : MonoBehaviour
{
    [SerializeField] private PlayerController player; 
    [SerializeField] private NavMeshSurface navMeshSurface;
    private bool isRebuilding;

    void OnEnable()
    {
        player.OnLanded += RequestRebuild;
    }

    void OnDisable()
    {
        player.OnLanded -= RequestRebuild;
    }

    public void RequestRebuild()
    {
        if (!isRebuilding)
            StartCoroutine(RebuildCoroutine());
    }

    IEnumerator RebuildCoroutine()
    {
        isRebuilding = true;

        // Destroy / Physics / Transform ‚Ì”½‰f‘Ò‚¿
        yield return null;

        navMeshSurface.BuildNavMesh();
        isRebuilding = false;
    }
}
