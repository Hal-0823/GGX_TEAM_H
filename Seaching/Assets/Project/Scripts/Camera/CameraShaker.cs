using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShaker : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    public void Shake()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        Debug.Log("Camera Shake Triggered");
        impulseSource.GenerateImpulse();
    }
}