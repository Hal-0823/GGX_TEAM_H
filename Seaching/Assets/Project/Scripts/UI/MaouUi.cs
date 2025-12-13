using UnityEngine;
using TMPro;
using System.Collections;

public class MaouUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f; // フェード時間

    private float timer;
    private bool isActive = false;
    private Coroutine fadeCoroutine;

    private readonly string[] messages = {
        "1",
        "2",
        "3",
        "4",
        "5"
    };

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // フェードアウト開始
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
            isActive = false;
        }
    }

    private void Start()
    {
        TriggerMaouMessage();
    }

    // テスト用
    public void TriggerMaouMessage()
    {
        messageText.text = messages[1];
        timer = displayTime;
        messagePanel.SetActive(true);

        // フェードイン開始
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));

        isActive = true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float time = 0f;
        cg.alpha = from;

        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        cg.alpha = to;

        // 完全に消えたらパネルを非表示にする
        if (to == 0f)
        {
            messageText.text = "";
            messagePanel.SetActive(false);
        }
    }
}
