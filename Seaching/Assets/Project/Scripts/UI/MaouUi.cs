using UnityEngine;
using TMPro;

public class MaouUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private int scoreThreshold = 500; // 500点ごとにお小言

    private float timer;
    private bool isActive = false;
    private int lastCheckedScore = 0;

    private string[] messages = {
        "1",
        "2",
        "3",
        "4",
        "5"
    };

    void Update()
    {
        // ScoreManagerから現在スコアを参照
        int currentScore = FindObjectOfType<ScoreManager>().CurrentTotalScore;

        // 閾値を超えたらお小言を出す
        if (currentScore >= lastCheckedScore + scoreThreshold)
        {
            ShowRandomMessage();
            lastCheckedScore = currentScore;
        }

        // 表示時間が過ぎたら消す
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                messageText.text = "";
                isActive = false;
            }
        }
    }

    private void ShowRandomMessage()
    {
        int index = Random.Range(0, messages.Length);
        messageText.text = messages[index];
        timer = displayTime;
        isActive = true;
    }
}
