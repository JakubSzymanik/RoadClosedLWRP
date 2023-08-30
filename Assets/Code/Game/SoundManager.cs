using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource slideSound;
    [SerializeField] private AudioSource connectionSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource dingSound;
    [SerializeField] private AudioSource successSound;

    public static SoundManager SoundManagerInstance { get; private set; }

    private void Awake()
    {
        SoundManagerInstance = this;
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1);
    }

    public void SwitchVolume()
    {
        float currentSound = AudioListener.volume;
        if (currentSound > 0)
        {
            PlayerPrefs.SetFloat("Volume", 0);
            AudioListener.volume = 0;
        }
        else
        {
            PlayerPrefs.SetFloat("Volume", 1);
            AudioListener.volume = 1;
        }
    }

    public void RequestSound(SoundType type)
    {
        if (AudioListener.volume == 0)
            return;
        switch (type)
        {
            case SoundType.Slide:
                slideSound.Play();
                break;
            case SoundType.Connection:
                connectionSound.Play();
                break;
            case SoundType.Explosion:
                explosionSound.Play();
                break;
            case SoundType.ButtonClick:
                buttonSound.Play();
                break;
            case SoundType.Ding:
                dingSound.Play();
                break;
            case SoundType.Success:
                successSound.Play();
                break;
        }
    }
}
