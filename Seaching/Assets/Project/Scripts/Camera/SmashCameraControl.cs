using UnityEngine;
using Unity.Cinemachine;

public class SmashCameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vCamNormal;
    [SerializeField] private CinemachineCamera vCamAiming;
    
    // プレイヤーの状態管理（例）
    public enum SmashState { Normal, Jumping, Aiming, Falling, Impact }
    
    public void UpdateCameraState(SmashState state)
    {
        if (state == SmashState.Aiming)
        {
            // 頂点付近で狙いをつけている時は、俯瞰カメラを優先
            vCamAiming.Priority = 15; 
            vCamNormal.Priority = 10;
        }
        else
        {
            // それ以外（通常、上昇、落下中）は通常カメラに戻す
            vCamAiming.Priority = 10;
            vCamNormal.Priority = 15;
        }
    }
}