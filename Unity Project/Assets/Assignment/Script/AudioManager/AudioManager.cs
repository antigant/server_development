using UnityEngine;
using UnityEngine.Audio;
using CustomPlugin;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;
    public static AudioManager instance;

    public AudioSource[] source;

    public AudioMixer mixer;

    //float[] volume = new float[3];
    CSound sound = new CSound();

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            source[0].playOnAwake = true;
            source[0].loop = true;
            source[1].playOnAwake = false;
            source[1].loop = false;

            //for (int i = 0; i < volume.Length; ++i)
            //    volume[i] = 0.0f;

            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);

        instance.PlayBGM(0, 0);
        instance.ResetVolume();
    }

    public void SetMasterVolume(float vol)
    {
        sound.Master = vol;
        mixer.SetFloat("master", sound.Master);
    }

    public void SetBgmVolume(float vol)
    {
        sound.Bgm = vol;
        mixer.SetFloat("bgm", sound.Bgm);
    }

    public void SetSfxVolume(float vol)
    {
        sound.Sfx = vol;
        mixer.SetFloat("sfx", sound.Sfx);
    }

    public CSound GetVolume()
    {
        return sound;
    }

    public float GetVolume(int index)
    {
        float[] vol = new float[3];
        vol[0] = sound.Master;
        vol[1] = sound.Bgm;
        vol[2] = sound.Sfx;

        return vol[index];
    }

    public void SetVolume(CSound sound)
    {
        this.sound = sound;
    }

    public void SetVolume(float[] vol)
    {
        sound.Master = vol[0];
        sound.Bgm = vol[1];
        sound.Sfx = vol[2];
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
