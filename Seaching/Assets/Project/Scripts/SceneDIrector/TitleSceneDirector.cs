using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleSceneDirector : MonoBehaviour
{
    //[SerializeField] private InputChannel inputChannel;
    [SerializeField] private RectTransform logoRect;       // ロゴのRectTransform
    [SerializeField] private CanvasGroup blackPanel;       // 暗転用の黒背景パネル
    [SerializeField] private RectTransform startTextRect;   // ボタンを押してスタート のテキスト

    private PlayerInput playerInput;

    private void OnEnable()
    {
        // 幅, 高さ, モード
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        playerInput = new PlayerInput();
        playerInput.Title.Next.performed += OnNextPressed;
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Dispose();
    }

    void Start()
    {
        Application.targetFrameRate = 90;
        blackPanel.alpha = 1.0f; // 最初は真っ黒
        // フェードイン
        blackPanel.DOFade(0.0f, 1.0f);
        AudioManager.Instance.PlayBGM("BGM_Title");
        // 0.8秒かけて 1.1倍 にし、それを繰り返す
        startTextRect.DOScale(1.1f, 0.8f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnNextPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        AudioManager.Instance.PlaySE("SE_Click6");

        startTextRect.DOKill(); // アニメーション停止
        //startTextRect.localScale = Vector3.zero; // スケールを0にして消す
        playerInput.Disable(); // 入力受付停止

        AudioManager.Instance.StopBGM(2.0f);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f);
        seq.Join(startTextRect.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f)); // スケール0にして消す
        // 1. 激しい振動（シェイク）
        // duration:時間, strength:揺れ幅, vibrato:振動数
        seq.Append(logoRect.DOShakeAnchorPos(3.0f, strength: 30, vibrato: 30));
        seq.JoinCallback(() => AudioManager.Instance.PlaySE("SE_Impact"));

        // // 2. 振動が終わったら一瞬で消す（スケール0にする）
        // seq.Append(logoRect.DOScale(Vector3.zero, 0.05f));
        
        // 3. 同時に暗転開始
        seq.Join(blackPanel.DOFade(1.0f, 2.5f));

        // 遷移
        seq.OnComplete(() => SceneManager.LoadScene("TutorialScene"));
    }
}
