using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class ResultSceneDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputChannel inputChannel;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] ScoreDisplay scoreDisplay;
    [SerializeField] private List<BreakableObject> buildings;
    [SerializeField] private PlayerController playerController;

    [Header("Building Appearance Settings")]
    [SerializeField] private float buildingAppearDuration = 0.4f;
    [SerializeField] private Ease buildingAppearEase = Ease.OutBack;

    private Dictionary<BreakableObject, Vector3> originalScales = new Dictionary<BreakableObject, Vector3>();
    private int currentBuildingIndex = 0;

    private void Awake()
    {
        scoreDisplay.OnRankUp += AppearBuildings;
        inputChannel.SwitchToNone();
        playerController.gameObject.SetActive(false);
        buildings.ForEach(b => originalScales[b] = b.transform.localScale);
    }

    private void Start()
    {
        buildings.ForEach(b => b.gameObject.SetActive(false));
        fadeCanvasGroup.alpha = 1f;
        StartCoroutine(ResultSequence());
    }

    private void OnDestroy()
    {
        scoreDisplay.OnRankUp -= AppearBuildings;
    }

    private IEnumerator ResultSequence()
    {
        // フェードイン
        yield return fadeCanvasGroup.DOFade(0f, 1f).WaitForCompletion();

        yield return new WaitForSeconds(1.0f);

        scoreDisplay.ShowResult();

        yield return new WaitUntil(() => scoreDisplay.IsCompleted);

        playerController.gameObject.SetActive(true);
        StartCoroutine(playerController.SmashActionSequence(1, 0));
    }
    
    /// <summary>
    /// 建物を出現させる
    /// </summary>
    private void AppearBuildings()
    {
        if (currentBuildingIndex >= buildings.Count) return;

        BreakableObject building = buildings[currentBuildingIndex];
        building.gameObject.SetActive(true);
        building.transform.localScale = Vector3.zero;

        building.transform.DOScale(originalScales[building], buildingAppearDuration)
            .SetEase(buildingAppearEase);

        currentBuildingIndex++;
    }
}   