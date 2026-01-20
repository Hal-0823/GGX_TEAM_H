using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 10f;
    [SerializeField] private float moveDuration = 5f;
    [SerializeField] private float targetZPosition = -10f;
    private async void Start()
    {
        await System.Threading.Tasks.Task.Delay((int)(delaySeconds * 1000));  // delaySeconds秒待機
        // Z軸方向にtargetZPositionまでmoveDuration秒かけて徐々に引く
        transform.DOLocalMoveZ(targetZPosition, moveDuration).SetEase(Ease.InOutSine);
    }
}