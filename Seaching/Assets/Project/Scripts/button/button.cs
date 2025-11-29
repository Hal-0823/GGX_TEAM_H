using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
public class startbutton_UI : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
        [SerializeField] protected CanvasGroup _canvasGroup; //Fadeとかを使う為に必要
        [SerializeField] protected Button _button;
        [SerializeField] protected TextMeshProUGUI _buttonText;//テキストを表示するためのもの
        private Image _image;
    
    void Awake()
    {   //値が入力されているか調べて、nullなら自動で割り当て
        if (_button == null) _button = GetComponent<Button>();
        if (_buttonText == null) _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
    }

    void Start()
    {
        if (_button != null)
        {
            _button.image.color = Color.blue;  // ボタンの背景色を青に
        }
        else
        {
            Debug.LogWarning("ButtonがInspectorで割り当てられていません");
        }

        if (_buttonText != null)
        {

            if (CompareTag("start"))
            {
            _buttonText.text = "start";    // ボタンの文字を変更
            }
            else if (CompareTag("result"))
            {
                _buttonText.text = "result";
            }    
        }
        else
        {
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(0.9f,0.24f).SetEase(Ease.OutCubic);//クリックされたらサイズを小さくする
        _canvasGroup?.DOFade(0.8f,0.24f).SetEase(Ease.OutCubic);//クリックされたら透明度を上げる
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerPress;

        transform.DOScale(1.0f,0.24f).SetEase(Ease.OutCubic);//クリックが離されたらサイズを大きくする
         _canvasGroup?.DOFade(1.0f,0.24f).SetEase(Ease.OutCubic);//クリックが離されたら透明度を下げる
        
        if (clickedObject.CompareTag("start"))
        {
            SceneManager.LoadScene("timertest");//タイマー画面に移行する 
        }
        else if (clickedObject.CompareTag("result"))
        {
            SceneManager.LoadScene("title");//タイトルに戻る
        }
        
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ゲーム開始");
    }
}