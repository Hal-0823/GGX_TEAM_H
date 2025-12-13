using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    void Start()
    {
        // 再生方式が変わったためコメントアウト
        //AudioManager.Instance.PlayBGM(clip);
    }

    void Update()
    {
        
    }
}
