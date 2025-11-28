using UnityEngine;

public class BravePersonController : MonoBehaviour
{
    public Transform player;            //プレイヤー.
    public float viewAngle = 45f;       //視野角（左右の角度）
    public float viewDistance = 10f;    //視野距離の限度
    public float moveSpeed = 3f;        //移動速度.

    private bool isChasing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが見えている場合.
        if (CanSeePlayer())
        {
            isChasing = true;
        }

        if (isChasing)
        {
            FindPlayer();
        }
    }

    //プレイヤーが見えるのか.
    bool CanSeePlayer()
    {
        //視野距離の限度より大きい場合、プレイヤーを見つけない.
        if (DistanceToPlayer() > viewDistance) return false;

        //正面との角度.
        float angle = Vector3.Angle(transform.forward, DirectionToPlayer());

        //視野角の範囲内の場合、見つけた.
        return angle < viewAngle;
    }

    //プレイヤーを見つけた時の動作.
    void FindPlayer()
    {
        if (DistanceToPlayer() > 2f)
        {
            //移動.
            transform.position += moveSpeed * Time.deltaTime * DirectionToPlayer();
        }

        //追跡中にプレイヤーの方を向く.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(DirectionToPlayer()),
            Time.deltaTime * 5f
        );
    }

    //プレイヤーへの距離.
    private float DistanceToPlayer()
    {
        Vector3 thisPos = transform.position;
        thisPos.y = 0f;
        Vector3 playerPos = player.position;
        playerPos.y = 0f;

        //プレイヤーとの差.
        Vector3 diff = playerPos - thisPos;
        //プレイヤーとの距離.
        float distance = diff.magnitude;

        return distance;
    }

    //プレイヤーへの方向.
    private Vector3 DirectionToPlayer()
    {
        Vector3 thisPos = transform.position;
        thisPos.y = 0f;
        Vector3 playerPos = player.position;
        playerPos.y = 0f;

        //プレイヤーとの差.
        Vector3 diff = playerPos - transform.position;
        //方向の正規化.
        Vector3 dir = diff.normalized;

        return dir;
    }
}
