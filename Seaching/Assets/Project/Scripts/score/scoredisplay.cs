using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreDisplay : MonoBehaviour
{
    public event Action OnRankUp;
    public bool IsCompleted => isCompleted;
    private bool isCompleted;

    [SerializeField] private GameSessionData sessionData;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI rankText;

    [Header("Count Up Settings")]
    [SerializeField] private float countUpSpeed = 5f;

    private bool isStarted = false;
    private int nowScore = 0;
    private int updateScore = 0;
    private string currentRank = "";


    public void ShowResult()//リザルト画面上に表示する
    {
        isStarted = true;
        //指定した値までカウントアップ
        DOTween.To(() => nowScore,(n) => nowScore = n,updateScore, updateScore / 10000 / countUpSpeed).OnUpdate(() => scoreText.text = "Score:" +nowScore.ToString("D6")).OnComplete(() => isCompleted = true);
    }

    private void rankdisplay()
    {
        string newRank = "";
        if (nowScore < 10000)//E
        {
            newRank = "F";
        }
        else if (10000 <= nowScore && nowScore < 20000)//E
        {
            newRank = "E";
        }
        else if (20000 < nowScore && nowScore < 30000)//D
        {
            newRank = "D";
        }
        else if (20000 <= nowScore && nowScore < 40000)//C
        {
            newRank = "C";
        }
        else if (40000 <= nowScore && nowScore < 60000)//B
        {
            newRank = "B";
        }
        else if (60000 < nowScore && nowScore < 80000)//A
        {
            newRank = "A";
        }
        else if (80000 < nowScore && nowScore < 90000)//S
        {
            newRank = "S";
        }
        else if (90000 < nowScore && nowScore < 99900)//SS
        {
            newRank = "SS";
        }
        else if (99900 < nowScore)//SSS
        {
            newRank = "SSS";
        }

        //ランクが変わった時だけ処理する
        if (newRank != currentRank)
        {
            OnRankUp?.Invoke();
            currentRank  = newRank;

            //色を変える
            switch (newRank)
            {
            case "E": rankText.DOColor(Color.gray, 0.1f); break;
            case "D": rankText.DOColor(Color.blue, 0.1f); break;
            case "C": rankText.DOColor(Color.green, 0.1f); break;
            case "B": rankText.DOColor(Color.red, 0.1f); break;
            case "A": rankText.DOColor(new Color(1f, 0.84f, 0f), 0.1f); break; // gold
            case "S": rankText.DOColor(new Color(0.70f, 0.78f, 0.90f), 0.1f); break; // blue-gold
            case "SS": rankText.DOColor(new Color(0.70f, 0.78f, 0.90f), 0.1f); break; // blue-gold
            case "SSS": rankText.DOColor(new Color(0.70f, 0.78f, 0.90f), 0.1f); break; // blue-gold
            }
        //テキストを更新
        rankText.text = newRank;

        //大きさを変えるアニメーション
        rankText.transform.DOScale(1.3f, 0.1f).From(1f).OnComplete(() =>
         {
             rankText.transform.DOScale(1f,0.1f);
         });
        }


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isStarted = false;
        nowScore = 0;
        scoreText.text = "Score:000000";
        rankText.text = "";
        updateScore = sessionData.currentScore;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted || nowScore >= updateScore) return;
        rankdisplay();
    }
}
