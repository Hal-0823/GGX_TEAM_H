using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float cameraInitialHeight = 22.5f;

    [SerializeField, Range(1,179)]
    private int min_FOV = 60;

    [SerializeField, Range(1,179)]
    private int max_FOV = 100;

    private Camera playerCamera;

    private float cameraHeight;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        playerCamera.fieldOfView = min_FOV;
        cameraHeight = cameraInitialHeight;
    }

    private void Update()
    {
        playerCamera.transform.position = new Vector3(
            playerTransform.position.x, 
            //cameraHeight,
            playerTransform.position.y + cameraInitialHeight,
            playerTransform.position.z
        );

        AdjustFOV(playerTransform.position.y);

    }

    public void AdjustFOV(float playerHeight)
    {
        float progress = playerHeight / cameraInitialHeight;
        if (progress > 0.9f) {
            cameraHeight = playerHeight + 1.5f;
        } else {
            cameraHeight = cameraInitialHeight;
        }
        progress = Mathf.Clamp01(progress);
        playerCamera.fieldOfView = Mathf.Lerp(min_FOV, max_FOV, progress);
    }
}
