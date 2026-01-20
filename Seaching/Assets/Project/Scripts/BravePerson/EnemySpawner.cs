using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;   //���O�ɃX�e�[�W�ɔz�u���ꂽ�X�|�[���|�C���g.
    public GameObject enemyPrefab;

    public float minSpawnDistance = 10f;  //�o������ŏ��l.
    public float maxSpawnDistance = 20f;  //�o������ő�l.

    private Transform player;
    private PlayerController playerController;
    private List<GameObject> enemies = new List<GameObject>();
    private int maxEnemies = 1;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        //playerController.OnLanded += SpawnEnemy;

        SpawnEnemy();
        InvokeRepeating(nameof(SpawnEnemy), 5f, 30f);
        InvokeRepeating(nameof(SpawnEnemy), 20f, 45f);
        InvokeRepeating(nameof(SpawnEnemy), 60f, 30f);
        InvokeRepeating(nameof(SpawnEnemy), 100f, 10f);
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.OnLanded -= SpawnEnemy;
        }

        CancelInvoke(nameof(SpawnEnemy));
    }

    //�X�|�[���������Ƃ��ɌĂ�.
    public void SpawnEnemy()
    {
        List<Transform> validPoints = new List<Transform>();
        
        foreach (var p in spawnPoints)
        {
            //�v���C���[�ƃX�|�[���|�C���g�̋���.
            float dist = Vector3.Distance(player.position, p.position);

            if (dist >= minSpawnDistance
                && dist <= maxSpawnDistance)
            {
                //�����ɍ����ꏊ������΃X�|�[��.
                validPoints.Add(p);
            }
        }

        //�����ɍ����X�|�[���n�_���Ȃ���ΏI��.
        if (validPoints.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, validPoints.Count);
        var enemy = Instantiate(enemyPrefab, validPoints[randomIndex].position, Quaternion.identity);
        enemies.Add(enemy);
    }
}
