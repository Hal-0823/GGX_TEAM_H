using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class Timer : MonoBehaviour
{
    [SerializeField]
    Text TimerText;
    public float limitTime;


    void Start()
    {
        
    }

    void Update()
    {
        limitTime -= Time.deltaTime;


        if (limitTime < 0)
        {
            limitTime = 0;
            SceneManager.LoadScene("result");
        }

        int minutes = Mathf.FloorToInt(limitTime / 60f);//分表示
        int seconds = Mathf.FloorToInt(limitTime % 60f);//秒表示

        TimerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        //残り時間を整数で表示
    }
}
