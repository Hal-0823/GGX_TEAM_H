using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    void Start()
    {
        AudioManager.Instance.PlayBGM(clip);
    }

    void Update()
    {
        
    }
}
