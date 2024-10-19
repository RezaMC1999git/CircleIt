using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Summary : A SingleTone To Controll Game's Music

    public static MusicPlayer Instance;
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);    
        }
    }

    private void Start()
    {
        PlayPauseMusic();
    }

    public void PlayPauseMusic() 
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
