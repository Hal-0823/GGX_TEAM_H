using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BravePersonController : MonoBehaviour
{
    public static event Action<int> OnBraveDefeated;
    [SerializeField] private int scoreValue = 100;

    private Transform        player;     //�v���C���[.
    private FoundPlayerUI    foundUI;    //����������UI.
  
    public float viewAngle = 45f;       //����p�i���E�̊p�x�j
    public float viewDistance = 10f;    //���싗���̌��x
    public float moveSpeed = 3f;        //�ړ����x.
    public float wanderRadius = 10f;    //���͂ɂǂꂾ��������.
    public float wanderTimerMax = 5f;      //���ɓ����܂ł̍ő厞��.

    private bool isChasing = false;

    private Animator anim;
    private NavMeshAgent agent;
    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        foundUI = GetComponentInChildren<FoundPlayerUI>(true);

        float walkTime = UnityEngine.Random.Range(0, wanderTimerMax);
        timer = walkTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled == false) return;
        //�v���C���[�������Ă���ꍇ.
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

    //�_���[�W���󂯂�.
    public void GetDamage(Vector3 explosionCenter, float power, float radius)
    {
        agent.enabled = false; // NavMeshAgentを無効化
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false; // 回転の固定を解除
        rb.AddExplosionForce(power, explosionCenter, radius, 3.0f);
        rb.AddTorque(Vector3.up * power * 0.1f, ForceMode.Impulse);

        OnBraveDefeated?.Invoke(scoreValue);

        // このオブジェクトを一定時間後に削除
        transform.DOScale(Vector3.zero, 2.0f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    //�v���C���[�̓���.
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

        return navHit.position;
    }

    //�v���C���[��������̂�.
    private bool CanSeePlayer()
    {
        //���싗���̌��x���傫���ꍇ�A�v���C���[�������Ȃ�.
        if (DistanceToPlayer() > viewDistance) return false;

        //���ʂƂ̊p�x.
        float angle = Vector3.Angle(transform.forward, DirectionToPlayer());

        //����p�͈͓̔��̏ꍇ�A������.
        return angle < viewAngle;
    }

    //�v���C���[�����������̓���.
    private void FindPlayer()
    {
        //�U�������ȊO�Ȃ�v���C���[��ǂ�.
        if (DistanceToPlayer() > 2f)
        {
            agent.SetDestination(player.position);  //NavMeshAgent�Œǐ�.
            anim.SetBool("IsAttack", false);
        }
        else
        {
            agent.ResetPath(); // �߂Â�����~�߂�
            anim.SetBool("IsAttack", true);
        }

        // �v���C���[�̕���������
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(DirectionToPlayer()),
            Time.deltaTime * 5f
        );
    }

    //�v���C���[�ւ̋���.
    private float DistanceToPlayer()
    {
        Vector3 thisPos = transform.position;
        thisPos.y = 0f;
        Vector3 playerPos = player.position;
        playerPos.y = 0f;

        //�v���C���[�Ƃ̍�.
        Vector3 diff = playerPos - thisPos;
        //�v���C���[�Ƃ̋���.
        float distance = diff.magnitude;

        return distance;
    }

    //�v���C���[�ւ̕���.
    private Vector3 DirectionToPlayer()
    {
        Vector3 thisPos = transform.position;
        thisPos.y = 0f;
        Vector3 playerPos = player.position;
        playerPos.y = 0f;

        //�v���C���[�Ƃ̍�.
        Vector3 diff = playerPos - thisPos;
        //�����̐��K��.
        Vector3 dir = diff.normalized;

        return dir;
    }

    //���E�����R�ɕ���.
    private void Wander()
    {
        timer += Time.deltaTime;

        //���Ԃ������玟�̓���.
        if (timer >= wanderTimerMax)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
        anim.SetBool("IsAttack", false);
    }
}
