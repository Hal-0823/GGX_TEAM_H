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
        // 0.8秒かけて 1.1倍 にし、それを繰り返す
        startTextRect.DOScale(1.1f, 0.8f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnNextPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        startTextRect.DOKill(); // アニメーション停止
        //startTextRect.localScale = Vector3.zero; // スケールを0にして消す
        playerInput.Disable(); // 入力受付停止

        Sequence seq = DOTween.Sequence();

        // 1. 激しい振動（シェイク）
        // duration:時間, strength:揺れ幅, vibrato:振動数
        seq.Append(logoRect.DOShakeAnchorPos(3.0f, strength: 50, vibrato: 50));

        // // 2. 振動が終わったら一瞬で消す（スケール0にする）
        // seq.Append(logoRect.DOScale(Vector3.zero, 0.05f));
        
        // 3. 同時に暗転開始
        seq.Join(blackPanel.DOFade(1.0f, 2.5f));

        // 遷移
        seq.OnComplete(() => SceneManager.LoadScene("TutorialScene"));
    }
}
