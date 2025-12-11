using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private InputChannel inputChannel;

    public Action OnSkip;

    [Header("UI Components")]
    [SerializeField] private GameObject dialogueBox; // 吹き出しの親オブジェクト
    [SerializeField] private TextMeshProUGUI dialogueText; // テキスト表示部

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f; // 文字送りの速さ
    [SerializeField] private int requiredClicksToSkip = 5; // スキップに必要なクリック数
    [SerializeField] private float skipResetTime = 0.5f; // スキップカウントのリセット時間

    // 内部変数
    private Queue<string> sentences = new Queue<string>(); // 会話文のキュー
    private string currentSentence; // 現在表示中の全文
    private bool isTyping = false; // 文字送り中かどうかのフラグ
    private Coroutine typingCoroutine; // コルーチン管理用

    private int currentClickCount = 0;
    private float lastClickTime = 0f;
    private bool isSkipped = false;

    // プレイヤーのInput制御用
    private PlayerInput playerInput;

    public bool IsDialgoueActive()
    {
        return dialogueBox.activeSelf;
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Dialogue.Next.performed += OnNextPressed;

        inputChannel.OnRequestPlayerControl += DisableControl;
        inputChannel.OnRequestDialogueControl += EnableControl;
        inputChannel.OnRequestNoneControl += DisableControl;

        dialogueText.text = "";
    }

    private void OnDisable()
    {
        playerInput.Dispose();

        inputChannel.OnRequestPlayerControl -= DisableControl;
        inputChannel.OnRequestDialogueControl -= EnableControl;
        inputChannel.OnRequestNoneControl -= DisableControl;
    }

    private void EnableControl()
    {
        playerInput.Dialogue.Enable();
    }

    private void DisableControl()
    {
        playerInput.Dialogue.Disable();
    }

    /// <summary>
    /// 会話を開始する（外部から呼ぶ）
    /// </summary>
    /// <param name="lines">会話文の配列</param>
    public void StartDialogue(string[] lines)
    {
        inputChannel.SwitchToDialogue();

        dialogueBox.SetActive(true);
        sentences.Clear();

        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// スキップの判定も含まれる
    /// </summary>
    private void OnNextPressed(InputAction.CallbackContext context)
    {
        // スキップ処理
        if (!isSkipped)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= skipResetTime)
            {
                currentClickCount++;
            }
            else
            {
                currentClickCount = 1; // リセットしてカウント開始
            }

            lastClickTime = Time.time;

            if (currentClickCount >= requiredClicksToSkip)
            {
                isSkipped = true;
                // スキップ処理
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = "";
                sentences.Clear();
                EndDialogue();
                OnSkip?.Invoke();
                return;
            }

            lastClickTime = Time.time;
        }

        // 文字送り or 次の文章へ
        if (isTyping)
        {
            // 文字送り中なら：全文を一気に表示してスキップ
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
        }
        else
        {
            // 文字送りが終わっていれば：次の文章へ
            DisplayNextSentence();
        }
    }

    private void DisplayNextSentence()
    {
        // 次の文章がなければ終了
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        
        // 文字送りコルーチン開始
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    // 文字送りのコルーチン
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // 1フレーム待つか、指定時間待つか
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        // 1. UIを消す
        dialogueBox.SetActive(false);

        // 3. ActionMapを「Player」に戻す（移動操作などの復活）
        inputChannel.SwitchToPlayer();
    }
}