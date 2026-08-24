using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource[] sfxSources;

    // NEW: store the current volume values
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    // Called by the BGM slider
    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        bgmSource.volume = value;
    }

    // Called by the SFX slider
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        foreach (var source in sfxSources)
        {
            source.volume = value;
        }
    }
}