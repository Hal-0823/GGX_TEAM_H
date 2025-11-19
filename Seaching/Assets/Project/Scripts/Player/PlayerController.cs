using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float jumpForce = 20f;

    [SerializeField]
    private SmashCameraControl smashCameraControl;

    private Vector2 movementInput;
    private Rigidbody playerRigidbody;
    private GroundChecker groundChecker;
    private PlayerInput playerInput;


    private bool IsGrounded()
    {
        return groundChecker.IsGrounded();
    }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
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
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            smashCameraControl.UpdateCameraState(SmashCameraControl.SmashState.Aiming);
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * moveSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + movement);
    }
}