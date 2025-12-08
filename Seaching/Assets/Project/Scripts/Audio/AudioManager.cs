using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    public void PlayBGM(AudioClip clip)
    {if (bgmSource.clip == clip )
        {
            return;
        }
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySE(AudioClip clip)
    {
        seSource.PlayOneShot(clip);
    }   

    public void StopBGM()
    {
        bgmSource.Stop();
    }

        public void StopSE()
    {
        seSource.Stop();
    }
}
