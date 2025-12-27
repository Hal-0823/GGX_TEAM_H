using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private Transform target;//近づきたい相手
    [SerializeField] private float itemspeed = 3f;//移動速度
    public float stopdistance = 2f;//この距離まで近づいたら実行


    void Start()
    {
        
    }

    void Update()
    {
       float distance =Vector3.Distance(transform.position, target.position);

       if (distance < stopdistance)//一定の距離まで近づいたら実行する
        {
            Mag();
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

    void OnTriggerEnter(Collider target)//破片とプレイヤーが触れた時の判定
    {
        Debug.Log("MPと触れたよ");
        Destroy(gameObject);
    }

       public void GetMagnet()
    {
        stopdistance = 7f;
    }


}
