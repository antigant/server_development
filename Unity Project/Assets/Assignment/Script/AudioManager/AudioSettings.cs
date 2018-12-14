using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider[] sliders;

	// Use this for initialization
	void Start ()
    {
        for (int i = 0; i < sliders.Length; ++i)
            sliders[i].value = AudioManager.instance.GetVolume(i);
        settingsPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetActive();
	}

    public void SetMasterVolume(float vol)
    {
        AudioManager.instance.SetMasterVolume(vol);
    }

    public void SetBgmVolume(float vol)
    {
        AudioManager.instance.SetBgmVolume(vol);
    }

    public void SetSfxVolume(float vol)
    {
        AudioManager.instance.SetSfxVolume(vol);
    }

    void SetActive()
    {
        settingsPanel.SetActive(!settingsPanel.GetActive());
    }
}
