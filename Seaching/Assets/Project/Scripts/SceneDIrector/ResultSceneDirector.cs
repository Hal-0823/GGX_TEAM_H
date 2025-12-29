using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ResultSceneDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputChannel inputChannel;
    [SerializeField] private Image fadePanel;
    [SerializeField] ScoreDisplay scoreDisplay;
    [SerializeField] private List<BreakableObject> buildings;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject secondSelectedButton;

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

        firstSelectedButton.SetActive(false);
        secondSelectedButton.SetActive(false);
    }

    private void Start()
    {
        buildings.ForEach(b => b.gameObject.SetActive(false));
        fadePanel.color = new Color(1, 1f, 1f, 1f); // 白
        StartCoroutine(ResultSequence());
    }

    private void OnDestroy()
    {
        scoreDisplay.OnRankUp -= AppearBuildings;
    }

    private IEnumerator ResultSequence()
    {
        // フェードイン
        fadePanel.DOFade(0f, 1.5f).OnComplete(() =>
        {
            fadePanel.gameObject.SetActive(false);
        });

        yield return new WaitForSeconds(2.0f);

        scoreDisplay.ShowResult();

        yield return new WaitUntil(() => scoreDisplay.IsCompleted);
        yield return new WaitForSeconds(1.1f);
        scoreDisplay.ConfirmRank();

        playerController.gameObject.SetActive(true);
        StartCoroutine(playerController.StompActionCoroutine(1, 0));
        yield return new WaitForSeconds(1.5f);

        AudioManager.Instance.PlayBGM("BGM_Result");

        firstSelectedButton.SetActive(true);
        secondSelectedButton.SetActive(true);

        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
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

        AudioManager.Instance.PlaySE("SE_Pop");
    }

    public void OnRetryButtonPressed()
    {
        AudioManager.Instance.PlaySE("SE_Click5");
        Transition("GameScene");
    }

    public void OnTitleButtonPressed()
    {
        AudioManager.Instance.PlaySE("SE_Click5");
        Transition("TitleScene");
    }

    private void Transition(string sceneName)
    {
        AudioManager.Instance.StopBGM(2.0f);
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0f, 0f, 0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Join(fadePanel.DOFade(1.0f, 3.0f).SetUpdate(true));
        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

}   