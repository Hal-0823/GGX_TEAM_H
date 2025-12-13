using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class TutorialSceneDirector : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private CinemachineCamera tutorialCamera;
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private GameObject maou;

    private bool buildingBreaked = false;
    private Coroutine tutorialCoroutine;
    private bool isFinished = false;

    public void OnBuildingBroken(int score)
    {
        buildingBreaked = true;
    }

    private void Awake()
    {
        BreakableObject.OnObjectBroken += OnBuildingBroken;
        dialogueManager.OnSkip += OnSkipTriggered;
        fadeCanvasGroup.alpha = 1f;
    }

    public void OnDestroy()
    {
        BreakableObject.OnObjectBroken -= OnBuildingBroken;
        dialogueManager.OnSkip -= OnSkipTriggered;
    }

    private void Start()
    {
        tutorialCamera.Priority = 15;
        tutorialCoroutine = StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        // フェードイン
        fadeCanvasGroup.DOFade(0f, 5f);

        //AudioManager.Instance.PlayBGM("BGM_Tutorial1");

        // チュートリアル会話の開始
        string[] tutorialLines = new string[]
        {
            "...で...というわけだ...",
            "...おい...きいているか...",
            "...むすこ...よ...",
            "...きいて...おるのか!?",
            "はぁ...まったく...",
            "いいか? もういちどだけ いうぞ。",
            "Spaceキー か Aボタンで ジャンプだ。",
            "まずは ちいさく ジャンプしてみろ。"
        };

        dialogueManager.StartDialogue(tutorialLines);

        yield return null;
        yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());

        tutorialCamera.Priority = 5;

        // ジャンプが終わるまで待機
        yield return new WaitUntil(() => !playerController.IsGrounded());
        Debug.Log("プレイヤーが地面から離れました", this);
        yield return new WaitUntil(() => playerController.IsGrounded());
        Debug.Log("プレイヤーが地面に着地しました", this);

        string[] followUpLines = new string[]
        {
            "よし、うまくやったな。",
            "つぎは ながく ジャンプするぞ。",
            "Spaceキー か Aボタンを\nおしっぱなしに してみろ。"
        };

        dialogueManager.StartDialogue(followUpLines);

        yield return null;
        yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());

        // ジャンプが終わるまで待機
        yield return new WaitUntil(() => !playerController.IsGrounded());
        Debug.Log("プレイヤーが地面から離れました", this);
        yield return new WaitUntil(() => playerController.IsGrounded());
        Debug.Log("プレイヤーが地面に着地しました", this);

        string[] concludingLines = new string[]
        {
            "よし、うまくやったな。",
            "ジャンプの あいだは じゆうに うごけるぞ",
            "ためしに あのたてもの まで とんで いってみろ。"
        };

        dialogueManager.StartDialogue(concludingLines);

        yield return null;
        yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());

        if (!buildingBreaked)
        {
            // 建物が壊されるまで待機
            yield return new WaitUntil(() => buildingBreaked);
            Debug.Log("建物が壊されました", this);
            string[] breakLines = new string[]
            {
                "よし、うまくやったな。",
                "たかい ジャンプ を すれば \n よりたくさん の ものを こわせるぞ"
            };

            dialogueManager.StartDialogue(breakLines);
            yield return null;
            yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());
        }
        else
        {
            Debug.Log("建物はすでに壊されています", this);

            string[] skipLines = new string[]
            {
                "って もう こわして しまっているではないか...",
                "まあ よい。"
            };
            dialogueManager.StartDialogue(skipLines);
            yield return null;
            yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());
        }

        // チュートリアル終了の会話
        string[] endingLines = new string[]
        {
            "これで とっくんは おわりだ。",
            "さあ、にんげんかい の \"せいち\" へ\nむかい すべて はかい するのだ!"
        };

        dialogueManager.StartDialogue(endingLines);

        yield return null;
        yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());

        StartCoroutine(FinishTutorial());
    }

    // チュートリアルのスキップ演出
    private void OnSkipTriggered()
    {
        if (isFinished) return;

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        StartCoroutine(SkipSequence());
    }

    private IEnumerator SkipSequence()
    {
        Debug.Log("チュートリアルスキップ開始", this);

        yield return new WaitForSeconds(1.0f);
        string[] skipLines = new string[]
        {
            "......む?",
            "はやく いきたくて うずうず しているな。",
            "よかろう!\"せいち\"の すべてを\nはかい するのだ!"
        };

        dialogueManager.StartDialogue(skipLines);
        yield return null;
        yield return new WaitUntil(() => !dialogueManager.IsDialgoueActive());

        StartCoroutine(FinishTutorial());
    }

    private IEnumerator FinishTutorial()
    {
        isFinished = true;

        tutorialCamera.Priority = 15;

        yield return new WaitForSeconds(1f);

        // 魔王を跳ねさせて、着地させる
        yield return maou.transform.DOLocalJump(maou.transform.localPosition + new Vector3(0, 0f, 0f), 100f, 1, 3.8f).WaitForCompletion();
        // 着地後、カメラに振動を加える
        cameraShaker.Shake();
        // プレイヤーは反動で画面外上まで飛ばされる
        playerController.gameObject.transform.DOLocalMoveY(200f, 5f).SetEase(Ease.OutQuad);
        fadeCanvasGroup.DOFade(1f, 3f);
        yield return new WaitForSeconds(3f);

        Debug.Log("チュートリアルシーケンス終了", this);

        // シーン遷移
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }
}