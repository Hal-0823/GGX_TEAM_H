using System.Collections;
using UnityEngine;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private InputChannel inputChannel;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        playerController.transform.position = startPoint.position;
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        // タイマー開始
        gameTimer.StartTimer();

        // プレイヤーの操作を有効化
        inputChannel.SwitchToPlayer();

        // プレイヤーの落下からスタート
        StartCoroutine(playerController.SmashActionSequence(0, 0f));

        // 地面に着地するまで待機
        yield return new WaitUntil(() => playerController.IsGrounded());    

        
    }
}