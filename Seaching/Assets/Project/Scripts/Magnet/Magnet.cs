using UnityEngine;

public class Magnet : MonoBehaviour
{
    private Transform target;//近づきたい相手
    [SerializeField] private float itemspeed = 15f;//移動速度
    private MagnetManager magnetmanager;

    void Start()
    {
       GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
       if (playerObj != null)
        {
            target = playerObj.transform;
        }

        magnetmanager = FindObjectOfType<MagnetManager>();
    }

    void Update()
    {
       if(target == null) return;
       if(magnetmanager == null) return;

       if (DistanceToTarget <= magnetmanager.draindistance)//一定の距離まで近づいたら実行する
        {
            Mag();
        }

        if(DistanceToTarget < 1f)
        {
            BrokenObjecttach();
            magnetmanager.GetMp();
        }
    }



    void Mag() //吸収される判定
    {
        transform.position = Vector3.MoveTowards(
        transform.position,
        target.position,
        itemspeed * Time.deltaTime
            );
    }

    public float DistanceToTarget
    {
        get
        {
            if(target == null) return float.MaxValue;
            return Vector3.Distance(transform.position, target.position);
        }
    }

    void BrokenObjecttach()//破片とプレイヤーが触れた時の判定
    {
        Debug.Log("MPと触れたよ");
        Destroy(gameObject);
    }




}
