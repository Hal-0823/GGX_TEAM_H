using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Audiotest : MonoBehaviour, IPointerEnterHandler
{
   [SerializeField] private AudioClip clip1;
   [SerializeField] private AudioClip clip2;
   [SerializeField] private Button myButton;
    void Start()
    {
        myButton.onClick.AddListener(PlaySE);
    }
    void PlaySE()
    {
        // 再生方式が変わったためコメントアウト
        //AudioManager.Instance.PlaySE(clip1);
        SceneManager.LoadScene("TestScene");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 再生方式が変わったためコメントアウト
        //AudioManager.Instance.PlaySE(clip2);
    }
}
