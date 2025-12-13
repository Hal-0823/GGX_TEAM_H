using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public Transform[] spawnPoints;   //���O�ɃX�e�[�W�ɔz�u���ꂽ�X�|�[���|�C���g.
    public GameObject enemyPrefab;

    public float minSpawnDistance = 10f;  //�o������ŏ��l.
    public float maxSpawnDistance = 20f;  //�o������ő�l.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    //�e�X�g�p.
    bool a = false;

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("�X�|�[���\\�Ȉʒu������܂���ł���");
            return;
        }

        //�����_����1��I��.
        Transform point = validPoints[Random.Range(0, validPoints.Count)];

        //�X�|�[��.
        Instantiate(enemyPrefab, point.position, point.rotation);
    }
}
