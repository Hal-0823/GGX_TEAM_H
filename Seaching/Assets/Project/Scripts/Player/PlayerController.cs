using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(StompAttack))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
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
    private StompAttack stompAttack;

    private Vector2 movementInput;

    [SerializeField]
    private bool isCharging = false;
    private float currentChargeTime = 0f;

    private Rigidbody rb;
    private Animator animator;
    private GroundChecker groundChecker;
    private PlayerInput playerInput;


    private bool IsGrounded()
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
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMovePerformed;
        playerInput.Player.Move.canceled -= OnMoveCanceled;
        playerInput.Player.Jump.started -= OnJumpTriggered;
        playerInput.Player.Jump.canceled -= OnJumpTriggered;
        playerInput.Disable();
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

    private void Update()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            UpdateVisuals(); // チャージ中の見た目更新
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
            Debug.Log("Lv3: 最大ジャンプ！");
            finalForce = jumpForceLv3;
        }
        else if (currentChargeTime >= stage1Threshold)
        {
            Debug.Log("Lv2: 中ジャンプ！");
            finalForce = jumpForceLv2;
        }
        else
        {
            Debug.Log("Lv1: 小ジャンプ");
            finalForce = jumpForceLv1;
        }

        StartCoroutine(SmashActionSequence(finalForce));
    }

    private void ResetCharge()
    {
        isCharging = false;
        currentChargeTime = 0f;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (currentChargeTime >= stage2Threshold)
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }
        else if (currentChargeTime >= stage1Threshold)
        {
            transform.localScale = new Vector3(1f, 0.75f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    // // 一連の動作を管理するコルーチン
    private IEnumerator SmashActionSequence(float jumpForce)
    {
        //isActionActive = true;

        // -----------------------------------------
        // 1. 上昇フェーズ (Jump)
        // -----------------------------------------
        // 上方向への初速を与える
        // 方向が入力されていた場合、その方向にも少し速度を与える
        rb.linearVelocity = Vector3.up * jumpForce + new Vector3(movementInput.x, 0, movementInput.y) * jumpForce / 10f;

        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Jumping);
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
        animator.SetTrigger("Fall");

        // カメラをエイムモードに切り替え
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Aiming);

        float timer = 0f;
        while (timer < hoverDuration)
        {
            timer += Time.deltaTime;


            // ボタンを離したら即落下などの処理を入れてもOK
            // if (Input.GetKeyUp(KeyCode.Space)) break;

            yield return null;
        }

        // -----------------------------------------
        // 3. 急降下フェーズ (Dive)
        // -----------------------------------------
        rb.useGravity = true; // 重力を戻す
        // 下方向に強力な初速を与える
        rb.linearVelocity = Vector3.down * diveSpeed;

        // カメラを落下モードに切り替え
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Falling);

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
        smashCameraControl.ShakeCamera();
        stompAttack.DoStomp();
        //isActionActive = false;
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Impact);

        yield return new WaitForSeconds(1.1f); // 少し待ってから通常モードへ
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Normal);
        animator.SetTrigger("Standup");
    }
}