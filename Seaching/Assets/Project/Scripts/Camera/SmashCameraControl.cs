using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CameraShaker))]
public class SmashCameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vCamNormal;
    [SerializeField] private CinemachineCamera vCamAiming;
    [SerializeField] private CinemachineCamera vCamFalling;
    [SerializeField] private CinemachineCamera vCamImpact;

    private CameraShaker cameraShaker;

    // プレイヤーの状態管理（例）
    public enum SmashState { Normal, Jumping, Aiming, Falling, Impact }

    public void UpdateCameraState(SmashState state)
    {
        vCamNormal.Priority = 5;
        vCamAiming.Priority = 5;
        vCamFalling.Priority = 5;
        vCamImpact.Priority = 5;

        switch (state)
        {
            case SmashState.Normal:
                vCamNormal.Priority = 10;
                break;
            case SmashState.Jumping:
                vCamAiming.Priority = 10;
                break;
            case SmashState.Aiming:
                vCamAiming.Priority = 10;
                break;
            case SmashState.Falling:
                vCamFalling.Priority = 10;
                break;
            case SmashState.Impact:
                vCamImpact.Priority = 10;
                break;
            default:
                vCamNormal.Priority = 10;
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