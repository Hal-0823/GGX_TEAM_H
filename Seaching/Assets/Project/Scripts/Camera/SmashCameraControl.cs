using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CameraShaker))]
public class SmashCameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vCamNormal;
    [SerializeField] private CinemachineCamera vCamAiming;
    [SerializeField] private CinemachineCamera vCamFalling;

    private CameraShaker cameraShaker;

    // プレイヤーの状態管理（例）
    public enum SmashState { Normal, Jumping, Aiming, Falling, Impact }

    public void UpdateCameraState(SmashState state)
    {
        switch (state)
        {
            case SmashState.Normal:
                vCamNormal.Priority = 10;
                vCamAiming.Priority = 5;
                vCamFalling.Priority = 5;
                break;
            case SmashState.Jumping:
                vCamNormal.Priority = 5;
                vCamAiming.Priority = 10;
                vCamFalling.Priority = 5;
                break;
            case SmashState.Aiming:
                vCamNormal.Priority = 5;
                vCamAiming.Priority = 10;
                vCamFalling.Priority = 5;
                break;
            case SmashState.Falling:
                vCamNormal.Priority = 5;
                vCamAiming.Priority = 5;
                vCamFalling.Priority = 10;
                break;
            default:
                vCamNormal.Priority = 10;
                vCamAiming.Priority = 5;
                vCamFalling.Priority = 5;
                break;
        }
    }

    public void ShakeCamera()
    {
        if (cameraShaker == null)
        {
            cameraShaker = GetComponent<CameraShaker>();
        }
        cameraShaker.Shake();
    }
}