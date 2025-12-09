using UnityEngine;
using UnityEngine.AI;

public class BravePersonController : MonoBehaviour
{
    public Transform        player;     //プレイヤー.
    public FoundPlayerUI    foundUI;    //見つけた時のUI.
  
    public float viewAngle = 45f;       //視野角（左右の角度）
    public float viewDistance = 10f;    //視野距離の限度
    public float moveSpeed = 3f;        //移動速度.
    public float wanderRadius = 10f;    //周囲にどれだけ動くか.
    public float wanderTimerMax = 5f;      //次に動くまでの最大時間.

    public float lifeMax = 10f;
    private float life = 0f;

    private bool isChasing = false;

    private Animator anim;
    private NavMeshAgent agent;
    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        life = lifeMax;

        agent = GetComponent<NavMeshAgent>();

        float walkTime = Random.Range(0, wanderTimerMax);
        timer = walkTime;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが見えている場合.
        if (CanSeePlayer())
        {
            isChasing = true;
            agent.speed = moveSpeed;
        }

        if (isChasing)
        {
            foundUI.FoundPlayer(this.gameObject);
            FindPlayer();
        }
        else
        {
            Wander();
        }
    }

    //ダメージを受ける.
    public void GetDamage(float damage)
    {
        life -= damage;

        if(life < 0)
        {
            Die();
        }
    }

    //プレイヤーの動き.
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

        return navHit.position;
    }

    //プレイヤーが見えるのか.
    private bool CanSeePlayer()
    {
        //視野距離の限度より大きい場合、プレイヤーを見つけない.
        if (DistanceToPlayer() > viewDistance) return false;

        //正面との角度.
        float angle = Vector3.Angle(transform.forward, DirectionToPlayer());

        //視野角の範囲内の場合、見つけた.
        return angle < viewAngle;
    }

    //プレイヤーを見つけた時の動作.
    private void FindPlayer()
    {
        //攻撃距離以外ならプレイヤーを追う.
        if (DistanceToPlayer() > 2f)
        {
            agent.SetDestination(player.position);  //NavMeshAgentで追跡.
            anim.SetBool("IsAttack", false);
        }
        else
        {
            agent.ResetPath(); // 近づいたら止める
            anim.SetBool("IsAttack", true);
        }

        // プレイヤーの方向を向く
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
        Vector3 diff = playerPos - thisPos;
        //方向の正規化.
        Vector3 dir = diff.normalized;

        return dir;
    }

    //世界を自由に歩む.
    private void Wander()
    {
        timer += Time.deltaTime;

        //時間がきたら次の動き.
        if (timer >= wanderTimerMax)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
        anim.SetBool("IsAttack", false);
    }

    //死亡時の動作.
    private void Die()
    {

    }
}
