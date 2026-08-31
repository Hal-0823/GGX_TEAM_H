using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField] private GameOverZone gameOverZone;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Image whitePanel;
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private InputChannel inputChannel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI timeUpText;

    private void Awake()
    {
        gameOverZone.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        gameOverZone.OnGameOver -= OnGameOver;
    }

    private void Start()
    {
        timeUpText.gameObject.SetActive(false);
        fadeCanvasGroup.alpha = 1f;
        whitePanel.gameObject.SetActive(false);
        playerController.transform.position = startPoint.position;
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        fadeCanvasGroup.DOFade(0f, 2f);

        inputChannel.SwitchToPlayer();

        // プレイヤーの落下からスタート
        StartCoroutine(playerController.StompActionCoroutine(0, 0f));

        // 地面に着地するまで待機
        yield return new WaitUntil(() => playerController.IsGrounded());

        AudioManager.Instance.PlaySE("SE_GameStart1");
        gameTimer.StartTimer();
        AudioManager.Instance.PlayBGM("BGM_Game1");
    }

    public void OnTimeOver()
    {
        StartCoroutine(OnTimeOverSequence());
    }

    private IEnumerator OnTimeOverSequence()
    {
        // 操作無効化
        inputChannel.SwitchToNone();
        playerController.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(0.5f);

        // タイムアップ表示
        timeUpText.gameObject.SetActive(true);
        timeUpText.text = "TIME UP!";
        timeUpText.transform.localScale = Vector3.zero;

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySE("SE_GameFinish");

        timeUpText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        // ヒットカウンターリセット
        HitCounterUI.instance.ForceReset();

        yield return new WaitForSeconds(2.5f);

        whitePanel.gameObject.SetActive(true);
        whitePanel.color = new Color(1, 1, 1, 0);

        Sequence seq = DOTween.Sequence();

        seq.Join(whitePanel.DOFade(1.0f, 0.5f).SetUpdate(true));

        seq.OnComplete(() =>
        {
            SceneManager.LoadScene("ResultScene");
        });
    }

    private void OnGameOver()
    {
        StartCoroutine(OnGameOverSequence());
    }
    
    private IEnumerator OnGameOverSequence()
    {
        // 操作無効化
        inputChannel.SwitchToNone();

        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySE("SE_GameFinish");

        // ヒットカウンターリセット
        HitCounterUI.instance.ForceReset();

        whitePanel.gameObject.SetActive(true);
        whitePanel.color = new Color(1, 1, 1, 0);

        Sequence seq = DOTween.Sequence();

        seq.Join(whitePanel.DOFade(1.0f, 0.5f).SetUpdate(true));

        seq.OnComplete(() =>
        {
            SceneManager.LoadScene("ResultScene");
        });
    }
}