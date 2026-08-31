using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;

public class TitleSceneDirector : MonoBehaviour
{
    //[SerializeField] private InputChannel inputChannel;
    [SerializeField] private RectTransform logoRect;       // ロゴのRectTransform
    [SerializeField] private CanvasGroup blackPanel;       // 暗転用の黒背景パネル
    [SerializeField] private RectTransform startTextRect;   // ボタンを押してスタート のテキスト

    [Header("Option")]
    [SerializeField] private RenderPipelineAsset highQualityPipeline; // 高品質レンダーパイプライン
    [SerializeField] private RenderPipelineAsset lowQualityPipeline;  // 低品質レンダーパイプライン

    private PlayerInput playerInput;

    private void OnEnable()
    {
        // 幅, 高さ, モード
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        playerInput = new PlayerInput();
        playerInput.Title.Next.performed += OnNextPressed;

        // GraphicsOption
        playerInput.Title.Option1.performed += OnOptionPressed;
        playerInput.Title.Option2.performed += OnOptionPressed;

        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Dispose();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        blackPanel.alpha = 1.0f; // 最初は真っ黒
        // フェードイン
        blackPanel.DOFade(0.0f, 1.0f);
        AudioManager.Instance.PlayBGM("BGM_Title");
        // 0.8秒かけて 1.1倍 にし、それを繰り返す
        startTextRect.DOScale(1.1f, 0.8f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);

        if (Gamepad.current != null)
        {
            startTextRect.GetComponent<TextMeshProUGUI>().text = "ボタンを押してスタート";
        } 
        else
        {
            startTextRect.GetComponent<TextMeshProUGUI>().text = "Spaceキーを押してスタート";
        }
    }

    private void OnNextPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        AudioManager.Instance.PlaySE("SE_Click6");

        startTextRect.DOKill();
        playerInput.Disable();

        AudioManager.Instance.StopBGM(2.0f);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f);
        seq.Join(startTextRect.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f));
        seq.Append(logoRect.DOShakeAnchorPos(3.0f, strength: 30, vibrato: 30));
        seq.JoinCallback(() => AudioManager.Instance.PlaySE("SE_Impact"));
        seq.Join(blackPanel.DOFade(1.0f, 2.5f));
        seq.OnComplete(() => SceneManager.LoadScene("TutorialScene"));
    }

    private void OnOptionPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        AudioManager.Instance.PlaySE("SE_Click6");

        switch (context.action.name)
        {
            case "Option1":
                QualitySettings.renderPipeline = highQualityPipeline;
                break;
            case "Option2":
                QualitySettings.renderPipeline = lowQualityPipeline;
                break;
        }
    }
}
