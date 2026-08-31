using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using DG.Tweening;
using NUnit.Framework;

[RequireComponent(typeof(BreakAttack))]
public class PlayerController : MonoBehaviour
{
    // 着地時のイベント
    public event Action OnLanded;

    [SerializeField] private InputChannel inputChannel;

    [Header("空中移動")]
    [SerializeField] private float baseHoverDuration = 1.5f;
    [SerializeField] private float baseMoveSpeedAir = 3f;
    [SerializeField] private float baseDiveSpeed = 30f;

    [Header("チャージ時間")]
    [Tooltip("1段階目のチャージ完了に必要な時間（秒）")]
    [SerializeField] private float baseStage1Threshold = 0.5f;
    [Tooltip("2段階目のチャージ完了に必要な時間（秒）")]
    [SerializeField] private float baseStage2Threshold = 1.5f;

    [Header("チャージ中の大きさ")]
    [SerializeField] private Vector3 baseChargeScaleLv1 = Vector3.one;
    [SerializeField] private Vector3 baseChargeScaleLv2 = new Vector3(1f, 0.75f, 1f);
    [SerializeField] private Vector3 baseChargeScaleLv3 = new Vector3(1f, 0.5f, 1f);

    [Header("ジャンプ力")]
    [SerializeField] private float baseJumpForceLv1 = 5f;  // 即押し・小ジャンプ
    [SerializeField] private float baseJumpForceLv2 = 10f; // 中ジャンプ
    [SerializeField] private float baseJumpForceLv3 = 15f; // 最大ジャンプ

    [Header("フィーバー中の強化倍率")]
    [SerializeField] private float feverMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float feverDiveSpeedMultiplier = 1.2f;
    [SerializeField] private float feverJumpForceMultiplier = 1.5f;
    [SerializeField] private float feverPlayerScaleMultiplier = 1.5f;

    [Header("勇者に当たった時の吹き飛び力")]
    [SerializeField] private float knockbackForce = 1000f;

    [SerializeField]
    private SmashCameraControl smashCameraControl;

    [SerializeField]
    private BreakAttack breakAttack;

    [SerializeField]
    private Renderer renderer;

    private Vector2 movementInput;

    private bool isCharging = false;
    private float currentChargeTime = 0f;
    private int currentJumpLevel = 0;
    private int lastJumpLevel = 0;
    private bool isJumping = false;
    private bool isStompTriggered = false;
    private bool isDiving = false;
    private bool isTakenDamage = false;

    private Rigidbody rb;
    private Animator animator;
    private GroundChecker groundChecker;
    private PlayerInput playerInput;

    // パラメータの取得プロパティ（フィーバー中は強化版を返す）
    private bool isFever = false;
    private float hoverDuration => isFever ? baseHoverDuration * feverMoveSpeedMultiplier : baseHoverDuration;
    private float moveSpeedAir => isFever ? baseMoveSpeedAir * feverMoveSpeedMultiplier : baseMoveSpeedAir;
    private float diveSpeed => isFever ? baseDiveSpeed * feverDiveSpeedMultiplier : baseDiveSpeed;
    private float stage1Threshold => isFever ? baseStage1Threshold * 0.8f : baseStage1Threshold;
    private float stage2Threshold => isFever ? baseStage2Threshold * 0.8f : baseStage2Threshold;
    private float jumpForceLv1 => isFever ? baseJumpForceLv1 * feverJumpForceMultiplier : baseJumpForceLv1;
    private float jumpForceLv2 => isFever ? baseJumpForceLv2 * feverJumpForceMultiplier : baseJumpForceLv2;
    private float jumpForceLv3 => isFever ? baseJumpForceLv3 * feverJumpForceMultiplier : baseJumpForceLv3;
    private Vector3 chargeScaleLv1 => isFever ? baseChargeScaleLv1 * feverPlayerScaleMultiplier : baseChargeScaleLv1;
    private Vector3 chargeScaleLv2 => isFever ? baseChargeScaleLv2 * feverPlayerScaleMultiplier : baseChargeScaleLv2;
    private Vector3 chargeScaleLv3 => isFever ? baseChargeScaleLv3 * feverPlayerScaleMultiplier : baseChargeScaleLv3;


    public bool IsGrounded()
    {
        return groundChecker.IsGrounded();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        groundChecker = GetComponentInChildren<GroundChecker>();
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;
        playerInput.Player.Jump.started += OnJumpTriggered;
        playerInput.Player.Jump.canceled += OnJumpTriggered;
        playerInput.Player.Stomp.started += OnStompTriggered;

        inputChannel.OnRequestPlayerControl += EnableControl;
        inputChannel.OnRequestDialogueControl += DisableControl;
        inputChannel.OnRequestNoneControl += DisableControl;

        FeverManager.OnFeverModeChanged += SetIsFever;
    }

    private void OnDisable()
    {
        playerInput.Dispose();
        inputChannel.OnRequestPlayerControl -= EnableControl;
        inputChannel.OnRequestDialogueControl -= DisableControl;
        inputChannel.OnRequestNoneControl -= DisableControl;
        FeverManager.OnFeverModeChanged -= SetIsFever;
    }

    private void EnableControl()
    {
        playerInput.Player.Enable();
    }

    private void DisableControl()
    {
        playerInput.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    private void OnJumpTriggered(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            if (context.started)
            {
                isCharging = true;
                currentChargeTime = 0f;
                currentJumpLevel = 1;
                lastJumpLevel = 1;
            }
            else if (context.canceled)
            {
                if (isCharging)
                {
                    Debug.Log("Jump Released");
                    ExcuteJump();
                    ResetCharge();
                }
            }
        }
    }

    private void OnStompTriggered(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isJumping)
            {
                Debug.Log("Stomp Triggered");
                isStompTriggered = true;
            }
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;

            if (currentChargeTime >= stage2Threshold)
            {
                currentJumpLevel = 3;
            }
            else if (currentChargeTime >= stage1Threshold)
            {
                currentJumpLevel = 2;
            }
            else
            {
                currentJumpLevel = 1;
            }

            if (currentJumpLevel != lastJumpLevel)
            {
                UpdateVisuals(currentJumpLevel);
                PlayChargeSE(currentJumpLevel);
                lastJumpLevel = currentJumpLevel;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsGrounded())
        {
            Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * moveSpeedAir * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void ExcuteJump()
    {
        float finalForce = 0f;

        // チャージ時間に応じた力の決定
        if (currentChargeTime >= stage2Threshold)
        {
            finalForce = jumpForceLv3;
            currentJumpLevel = 3;
            AudioManager.Instance.PlaySE("SE_JumpBig");
        }
        else if (currentChargeTime >= stage1Threshold)
        {
            finalForce = jumpForceLv2;
            currentJumpLevel = 2;
            AudioManager.Instance.PlaySE("SE_JumpMid");
        }
        else
        {
            finalForce = jumpForceLv1;
            currentJumpLevel = 1;
            AudioManager.Instance.PlaySE("SE_JumpSmall1");
        }

        StartCoroutine(StompActionCoroutine(currentJumpLevel, finalForce));
    }

    private void ResetCharge()
    {
        isCharging = false;
        currentChargeTime = 0f;
        currentJumpLevel = 1;
        lastJumpLevel = 1;
        UpdateVisuals(1);
    }

    private void UpdateVisuals(int jumpLevel)
    {
        switch (jumpLevel)
        {
            case 1: transform.localScale = chargeScaleLv1; break;
            case 2: transform.localScale = chargeScaleLv2; break;
            case 3: transform.localScale = chargeScaleLv3; break;
        }
    }

    private void PlayChargeSE(int jumpLevel)
    {
        switch (jumpLevel)
        {
            case 2: AudioManager.Instance.PlaySE("SE_Charge2", 0.65f); break;
            case 3: AudioManager.Instance.PlaySE("SE_Charge2", 1.1f); break;
        }
    }

    public IEnumerator StompActionCoroutine(int jumpLevel, float jumpForce)
    {
        Coroutine jumpCoroutine = StartCoroutine(JumpCoroutine(jumpLevel, jumpForce));

        // ジャンプが完了するか、ストンプがトリガーされるまで待機
        yield return new WaitUntil(() => isJumping == false || isStompTriggered == true);

        // ストンプがトリガーされた場合、ジャンプコルーチンを停止
        if (isJumping && isStompTriggered)
        {
            StopCoroutine(jumpCoroutine);
            isJumping = false;
            isStompTriggered = false;
        }

        Coroutine diveCoroutine = StartCoroutine(DiveCoroutine(jumpLevel));

        // 急降下が完了するか、ダメージを受けるまで待機
        yield return new WaitUntil(() => isDiving == false || isTakenDamage == true);

        if (isDiving && isTakenDamage)
        {
            StopCoroutine(diveCoroutine);
            isDiving = false;
        }

    }

    private IEnumerator JumpCoroutine(int jumpLevel, float jumpForce)
    {
        isJumping = true;
        if (HitCounterUI.instance != null)
        {
            HitCounterUI.instance.ForceReset();
        }

        rb.linearVelocity = Vector3.up * jumpForce;

        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Jumping);
        }

        while (rb.linearVelocity.y > 0.5f)
        {
            yield return null;
        }

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        // カメラをエイムモードに切り替え
        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Aiming);
        }

        if (jumpLevel >= 1)
        {
            float timer = 0f;
            while (timer < hoverDuration)
            {
                timer += Time.deltaTime;

                yield return null;
            }
        }
        isJumping = false;
    }

    private IEnumerator DiveCoroutine(int jumpLevel)
    {
        isDiving = true;
        Debug.Log("Dive Start");

        // 落下を開始した時点で前のトリガーが残っていたらリセットする
        animator.ResetTrigger("Land");
        animator.ResetTrigger("Standup");
        
        animator.SetTrigger("Fall");
        rb.useGravity = true;
        rb.linearVelocity = Vector3.down * diveSpeed;

        // カメラを落下モードに切り替え
        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Falling);
        }

        while (!IsGrounded())
        {
            yield return null;
        }

        animator.SetTrigger("Land");
        OnLanded?.Invoke(); // 着地イベントを発火

        if (smashCameraControl != null)
        {
            smashCameraControl.ShakeCamera();
        }
        StartCoroutine(breakAttack.DoStompCoroutine(jumpLevel));

        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Impact);
        }

        yield return new WaitForSeconds(1.1f); // 少し待ってから通常モードへ
        
        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Normal);
        }
        animator.SetTrigger("Standup");
        isDiving = false;
    }

    
    public void OnCollisionEnter(Collision collision)
    {
        // 空中にいるとき
        if (!IsGrounded())
        {
            // 建物に衝突した場合
            if (collision.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                breakAttack.DoBreak(3f, false);
            }
        }
        
        // 敵に衝突した場合
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TakenDamage(collision.transform.position);
        }
        
    }

    public void EnablePlayerControl()
    {
        inputChannel.SwitchToPlayer();
    }

    private void SetIsFever(bool value)
    {
        isFever = value;
        UpdateVisuals(currentJumpLevel);
    }

    public void TakenDamage(Vector3 enemyPos)
    {
        // 空中にいるとき、ダメージを受けているとき、フィーバー中は無効
        if (!IsGrounded() || isTakenDamage || isFever) return;
        AudioManager.Instance.PlaySE("SE_Land3");
        isTakenDamage = true;
        animator.SetTrigger("Standup");
        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Normal);
        }
        // 吹っ飛び処理
        // 敵から自分への方向を計算
        Vector3 awayFromEnemy = (transform.position - enemyPos).normalized;
        Vector3 knockbackDir = (awayFromEnemy + Vector3.up * 0.8f).normalized;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

        ResetCharge();

        // 点滅させる
        HitBlink();
    }

    private void HitBlink()
    {
        Sequence blinkSequence = DOTween.Sequence();
        blinkSequence.AppendCallback(() => renderer.enabled = false);
        blinkSequence.AppendInterval(0.07f);
        blinkSequence.AppendCallback(() => renderer.enabled = true);
        blinkSequence.AppendInterval(0.07f);
        blinkSequence.SetLoops(4);
        blinkSequence.AppendCallback(() => isTakenDamage = false);
        blinkSequence.Play();
    }
}