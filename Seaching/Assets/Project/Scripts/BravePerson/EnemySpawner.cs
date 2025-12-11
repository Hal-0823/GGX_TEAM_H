using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public Transform[] spawnPoints;   //事前にステージに配置されたスポーンポイント.
    public GameObject enemyPrefab;

    public float minSpawnDistance = 10f;  //出現する最小値.
    public float maxSpawnDistance = 20f;  //出現する最大値.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    //テスト用.
    bool a = false;

    // Update is called once per frame
    void Update()
    {
    }

    //スポーンしたいときに呼ぶ.
    public void SpawnEnemy()
    {
        List<Transform> validPoints = new List<Transform>();
        
        foreach (var p in spawnPoints)
        {
            //プレイヤーとスポーンポイントの距離.
            float dist = Vector3.Distance(player.position, p.position);

            if (dist >= minSpawnDistance
                && dist <= maxSpawnDistance)
            {
                //条件に合う場所があればスポーン.
                validPoints.Add(p);
            }
        }

        //条件に合うスポーン地点がなければ終了.
        if (validPoints.Count == 0)
        {
            Debug.Log("スポーン可能な位置がありませんでした");
            return;
        }

        //ランダムに1つを選ぶ.
        Transform point = validPoints[Random.Range(0, validPoints.Count)];

        //スポーン.
        Instantiate(enemyPrefab, point.position, point.rotation);
    }
}
