using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }
}