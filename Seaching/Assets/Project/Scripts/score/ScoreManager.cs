using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    public static ScoreManager Instance;
    private int score = 100000;

    private void Awake()
    {
        if (Instance == null)//シーンを跨いでも値を破棄しない
        {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()//score加算
    {
        BreakableObject.OnObjectBroken += AddScore;
    }

        private void OnDisable()//score減少
    {
        BreakableObject.OnObjectBroken -= AddScore;
    }

    private void AddScore(int value)//加算したスコアを表示する
    {
        score += value;
        scoreText.text = "Score:"+score;
        Debug.Log("Score:"+ score);
    }

    public int GetScore()
    {
        return score;
    }


    void Start()
    {

    }
    void Update()
    {
    }
}

