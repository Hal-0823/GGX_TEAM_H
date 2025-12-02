using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 20f;
    [SerializeField]
    private float hoverDuration = 1.5f;
    [SerializeField]
    private float moveSpeedAir = 3f;
    [SerializeField]
    private float diveSpeed = 30f;

    [SerializeField]
    private SmashCameraControl smashCameraControl;

    [SerializeField]
    private StompAttack stompAttack;

    private Vector2 movementInput;
    private Rigidbody rb;
    private GroundChecker groundChecker;
    private PlayerInput playerInput;


    private bool IsGrounded()
    {
        return groundChecker.IsGrounded();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponentInChildren<GroundChecker>();
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;
        playerInput.Player.Jump.started += OnJumpTriggered;
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMovePerformed;
        playerInput.Player.Move.canceled -= OnMoveCanceled;
        playerInput.Player.Jump.started -= OnJumpTriggered;
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
            StartCoroutine(SmashActionSequence());
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Jumping);
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    // 一連の動作を管理するコルーチン
    private IEnumerator SmashActionSequence()
    {
        //isActionActive = true;

        // -----------------------------------------
        // 1. 上昇フェーズ (Jump)
        // -----------------------------------------
        rb.linearVelocity = Vector3.up * jumpForce;

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
        smashCameraControl.ShakeCamera();
        stompAttack.DoStomp();
        //isActionActive = false;
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Impact);
        
        yield return new WaitForSeconds(1.1f); // 少し待ってから通常モードへ
        smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Normal);
    }
}