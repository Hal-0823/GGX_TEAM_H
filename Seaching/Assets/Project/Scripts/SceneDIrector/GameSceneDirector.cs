using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private InputChannel inputChannel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI timeUpText;

    private void Start()
    {
        timeUpText.gameObject.SetActive(false);
        fadeCanvasGroup.alpha = 1f;
        playerController.transform.position = startPoint.position;
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        // フェードイン
        fadeCanvasGroup.DOFade(0f, 2f);

        // プレイヤーの操作を有効化
        inputChannel.SwitchToPlayer();

        // プレイヤーの落下からスタート
        StartCoroutine(playerController.SmashActionSequence(0, 0f));

        // 地面に着地するまで待機
        yield return new WaitUntil(() => playerController.IsGrounded());

        // タイマー開始
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

        // タイムアップ表示
        timeUpText.gameObject.SetActive(true);
        timeUpText.text = "TIME UP!";
        timeUpText.transform.localScale = Vector3.zero;

        AudioManager.Instance.PlaySE("SE_GameFinish");

        yield return new WaitUntil(() => timeUpText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).IsComplete());

        // ヒットカウンターリセット
        HitCounterUI.instance.ForceReset();

        yield return new WaitForSeconds(2f);

        // リザルト表示

    }
}