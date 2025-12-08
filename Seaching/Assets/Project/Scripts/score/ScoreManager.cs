using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    private int score = 0;

    private void OnEnable()
    {
        BreakableObject.OnObjectBroken += AddScore;
    }

        private void OnDisable()
    {
        BreakableObject.OnObjectBroken -= AddScore;
    }

    private void AddScore(int value)
    {
        score += value;
        scoreText.text = "Score:"+score;
        Debug.Log("Score:"+ score);
    }

    void Start()
    {
        
    }
    void Update()
    {
    }
}

