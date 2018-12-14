using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;
    public static AudioManager instance;

    public AudioSource[] source;

    public AudioMixer mixer;

    float[] volume = new float[3];

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

            for (int i = 0; i < volume.Length; ++i)
                volume[i] = 0.0f;

            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
            Destroy(gameObject);

        instance.PlayBGM(0, 0);
        instance.ResetVolume();
    }

    public void SetMasterVolume(float vol)
    {
        volume[0] = vol;
        mixer.SetFloat("master", vol);
    }

    public void SetBgmVolume(float vol)
    {
        volume[1] = vol;
        mixer.SetFloat("bgm", vol);
    }

    public void SetSfxVolume(float vol)
    {
        volume[2] = vol;
        mixer.SetFloat("sfx", vol);
    }

    public float GetVolume(int index)
    {
        return volume[index];
    }

    public void SetVolume(float[] vol)
    {
        volume = vol;
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

    void ResetVolume()
    {
        SetMasterVolume(0.0f);
        SetBgmVolume(0.0f);
        SetSfxVolume(0.0f);
    }
}
