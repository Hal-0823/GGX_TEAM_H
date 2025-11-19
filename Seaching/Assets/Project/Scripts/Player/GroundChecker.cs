using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float rayLength = 1.0f;
    [SerializeField]
    private Vector3 rayStartOffset = new Vector3(0, 0.05f, 0);

    public bool IsGrounded()
    {
        // Rayの発射位置
        Vector3 rayStart = transform.position + rayStartOffset;
        // Rayの方向ベクトル (真下へ rayLength 分の長さ)
        Vector3 rayDirection = Vector3.down * rayLength;
        
        // レイをシーンビューに描画して可視化する
        Debug.DrawRay(
            rayStart, 
            rayDirection, 
            Color.green,    // レイの色
            1000f           // 描画時間
        );

        // 実際のRaycast判定
        return Physics.Raycast(
            rayStart,
            Vector3.down,
            rayLength,
            groundLayer
        );
    }
}