using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // これが必須
using DG.Tweening;

public class UISelectAnimator : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("アニメーション設定")]
    [SerializeField] private float scaleAmount = 1.1f; // 拡大率
    [SerializeField] private float duration = 0.2f;    // かかる時間
    [SerializeField] private bool useLoop = false;     // ふわふわし続けるか？

    private Vector3 originalScale;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlaySelectAnimation();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        PlayDeselectAnimation();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 選択状態でない場合のみアニメーション
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            PlaySelectAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            PlayDeselectAnimation();
        }
    }

    private void PlaySelectAnimation()
    {
        AudioManager.Instance.PlaySE("SE_Select");

        rectTransform.DOKill();
        
        if (useLoop)
        {
            rectTransform.DOScale(scaleAmount, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
        else
        {
            rectTransform.DOScale(scaleAmount, duration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
    }

    private void PlayDeselectAnimation()
    {
        rectTransform.DOKill();
        rectTransform.DOScale(originalScale, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }
    
    // オブジェクトが無効になったらアニメーションを止めてリセット
    void OnDisable()
    {
        rectTransform.DOKill();
        rectTransform.localScale = originalScale;
    }
}