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

    // --- インターフェース実装部分 ---

    // 1. コントローラー/キーボードで「選択」されたとき
    public void OnSelect(BaseEventData eventData)
    {
        PlaySelectAnimation();
    }

    // 2. 選択が外れたとき
    public void OnDeselect(BaseEventData eventData)
    {
        PlayDeselectAnimation();
    }

    // 3. マウスカーソルが乗ったとき（Hover）
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 選択状態でない場合のみアニメーション（二重実行防止）
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            PlaySelectAnimation();
        }
    }

    // 4. マウスカーソルが離れたとき
    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            PlayDeselectAnimation();
        }
    }

    // --- アニメーション処理 ---

    private void PlaySelectAnimation()
    {
        AudioManager.Instance.PlaySE("SE_Select");

        // アニメーション
        rectTransform.DOKill(); // 前の動きをキャンセル
        
        if (useLoop)
        {
            // ふわふわループさせる場合
            rectTransform.DOScale(scaleAmount, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true); // ポーズ中でも動くように
        }
        else
        {
            // パッと大きくする場合
            rectTransform.DOScale(scaleAmount, duration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
    }

    private void PlayDeselectAnimation()
    {
        // 元に戻す
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