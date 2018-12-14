using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;
    public static AudioManager instance;

    public AudioSource[] source;

	// Use this for initialization
	void Awake()
    {
        if(instance == null)
        {
            instance = this;

            source[0].playOnAwake = true;
            source[0].loop = true;
            source[1].playOnAwake = false;
            source[1].loop = false;

            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
            Destroy(gameObject);

        instance.PlayBGM(0, 0);
    }

    public void PlayBGM(int index, ulong timeDelay = 0)
    {
        source[0].clip = bgmClips[index];
        source[0].Play(timeDelay);
    }

    public void PlaySFX(int index, ulong timeDelay = 0)
    {
        source[1].clip = sfxClips[index];
        source[1].Play(timeDelay);
    }
}
