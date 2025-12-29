using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using System;

[RequireComponent(typeof(BreakAttack))]
public class PlayerController : MonoBehaviour
{
    // 着地時のイベント
    public event Action OnLanded;

    [SerializeField] private InputChannel inputChannel;

    [SerializeField] private float hoverDuration = 1.5f;
    [SerializeField] private float moveSpeedAir = 3f;
    [SerializeField] private float diveSpeed = 30f;

    [Header("Charge Settings")]
    [Tooltip("1段階目のチャージ完了に必要な時間（秒）")]
    [SerializeField] private float stage1Threshold = 0.5f;
    [Tooltip("2段階目のチャージ完了に必要な時間（秒）")]
    [SerializeField] private float stage2Threshold = 1.5f;

    [Header("ジャンプ力")]
    [SerializeField] private float jumpForceLv1 = 5f;  // 即押し・小ジャンプ
    [SerializeField] private float jumpForceLv2 = 10f; // 中ジャンプ
    [SerializeField] private float jumpForceLv3 = 15f; // 最大ジャンプ

    [SerializeField]
    private SmashCameraControl smashCameraControl;

    [SerializeField]
    private BreakAttack breakAttack;

    private Vector2 movementInput;

    private bool isCharging = false;
    private float currentChargeTime = 0f;
    private int currentJumpLevel = 0;
    private int lastJumpLevel = 0;
    private bool isJumping = false;
    private bool isStompTriggered = false;

    private Rigidbody rb;
    private Animator animator;
    private GroundChecker groundChecker;
    private PlayerInput playerInput;


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
    }

    private void OnDisable()
    {
        playerInput.Dispose();
        inputChannel.OnRequestPlayerControl -= EnableControl;
        inputChannel.OnRequestDialogueControl -= DisableControl;
        inputChannel.OnRequestNoneControl -= DisableControl;
    }

    // プレイヤーの操作を有効化
    private void EnableControl()
    {
        playerInput.Player.Enable();
    }

    // プレイヤーの操作を無効化
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
            // rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
            //smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Jumping);
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
                UpdateVisuals(currentJumpLevel); // チャージ中の見た目更新
                PlayChargeSE(currentJumpLevel); // チャージ音再生
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
            case 1: transform.localScale = Vector3.one; break;
            case 2: transform.localScale = new Vector3(1f, 0.75f, 1f); break;
            case 3: transform.localScale = new Vector3(1f, 0.5f, 1f); break;
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

    // // 一連の動作を管理するコルーチン
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

        yield return StartCoroutine(DiveCoroutine(jumpLevel));
    }

    private IEnumerator JumpCoroutine(int jumpLevel, float jumpForce)
    {
        isJumping = true;
        if (HitCounterUI.instance != null)
        {
            HitCounterUI.instance.ForceReset();
        }
        //isActionActive = true;

        // -----------------------------------------
        // 1. 上昇フェーズ (Jump)
        // -----------------------------------------
        // 上方向への初速を与える
        rb.linearVelocity = Vector3.up * jumpForce;

        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Jumping);
        }
        // 上昇中は待機（Y速度が落ちてくるまで、または一定時間）
        // ここでは簡易的に「Y速度が0に近づくまで」待ちます
        while (rb.linearVelocity.y > 0.5f)
        {
            yield return null;
        }

        // -----------------------------------------
        // 2. 滞空・狙いフェーズ (Hover & Aim)
        // -----------------------------------------
        // 物理演算の「嘘」をつくパート
        rb.useGravity = false;    // 重力を切る
        rb.linearVelocity = Vector3.zero; // 慣性を消してピタッと止める

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
        Debug.Log("Dive Start");
        animator.SetTrigger("Fall");
        // -----------------------------------------
        // 3. 急降下フェーズ (Dive)
        // -----------------------------------------
        rb.useGravity = true; // 重力を戻す
        // 下方向に強力な初速を与える
        rb.linearVelocity = Vector3.down * diveSpeed;

        // カメラを落下モードに切り替え
        if (smashCameraControl != null)
        {
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Falling);
        }

        // 着地するまで待機（接地判定はCollisionなどを利用しても良い）
        // 今回は簡易的に「Y座標が一定以下」または「速度が0になる」までとします
        // 本来は OnCollisionEnter で着地エフェクトを出してフラグを折るのがベスト
        while (!IsGrounded())
        {
            yield return null;
        }

        // -----------------------------------------
        // 終了処理
        // -----------------------------------------
        // 着地時の振動や破壊処理をここで呼ぶ
        animator.SetTrigger("Land");
        OnLanded?.Invoke(); // 着地イベントを発火

        if (smashCameraControl != null)
        {
            smashCameraControl.ShakeCamera();
        }
        StartCoroutine(breakAttack.DoStompCoroutine(jumpLevel));
        //isActionActive = false;
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
    }

    // 落下中に建物に衝突した場合の処理
    public void OnCollisionEnter(Collision collision)
    {
        if (IsGrounded()) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Debris"))
        {
            return;
        }
        breakAttack.DoBreak(1f, false);
    }

    public void EnablePlayerControl()
    {
        inputChannel.SwitchToPlayer();
    }
}