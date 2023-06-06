using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public float bgmVolume;
    public float sfxVolume;

    public AudioMixer mixer;

    public float volumeInterval;

    public GameObject bgmSoundBar;
    public GameObject sfxSoundBar;

    private Dictionary<string, AudioSource> sounds = new Dictionary<string, AudioSource>();

    void Start()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            sounds.Add(transform.GetChild(0).GetChild(i).name, transform.GetChild(0).GetChild(i).GetComponent<AudioSource>());
        }
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            sounds.Add(transform.GetChild(1).GetChild(i).name, transform.GetChild(1).GetChild(i).GetComponent<AudioSource>());
        }
        UpdateSFXVolume(sfxVolume);
        UpdateBGMVolume(bgmVolume);
    }

    void Update()
    {
    }
    public void IncreaseBGMVolume()
    {
        if (bgmVolume >= 1)
        {
            bgmVolume = 1;
            return;
        }
        if (bgmVolume == 0.001f)
        {
            bgmVolume = 0;
        }
        bgmVolume += volumeInterval;
        UpdateBGMVolume(bgmVolume);
    }
    public void DecreaseBGMVolume()
    {
        if (bgmVolume == 0.001f)
        {
            return;
        }
        bgmVolume -= volumeInterval;
        if (bgmVolume < 0.001f)
        {
            bgmVolume = 0.001f;
        }
        UpdateBGMVolume(bgmVolume);
    }
    public void IncreaseSFXVolume()
    {
        if (sfxVolume >= 1)
        {
            sfxVolume = 1;
            return;
        }
        if (sfxVolume == 0.001f)
        {
            sfxVolume = 0;
        }
        sfxVolume += volumeInterval;
        UpdateSFXVolume(sfxVolume);
    }
    public void DecreaseSFXVolume()
    {
        if (sfxVolume == 0.001f)
        {
            return;
        }
        sfxVolume -= volumeInterval;
        if (sfxVolume < 0.001f)
        {
            sfxVolume = 0.001f;
        }
        UpdateSFXVolume(sfxVolume);
    }
    public void UpdateBGMVolume(float volume)
    {
        mixer.SetFloat("bgmVolume", Mathf.Log10(volume) * 20);
        UpdateSoundBars();
        bgmVolume = volume;
    }
    public void UpdateSFXVolume(float volume)
    {
        mixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        UpdateSoundBars();
        sfxVolume = volume;
    }
    public void UpdateSoundBars()
    {
        for (int i = 0; i < 10; i++)
        {
            bgmSoundBar.transform.GetChild(i).gameObject.SetActive(false);
            sfxSoundBar.transform.GetChild(i).gameObject.SetActive(false);
            bgmVolume = (float)decimal.Round((decimal)bgmVolume, 1, System.MidpointRounding.ToEven);
            sfxVolume = (float)decimal.Round((decimal)sfxVolume, 1, System.MidpointRounding.ToEven);
        }
        for (int i = 0; i < Mathf.Round(bgmVolume * 10); i++)
        {
            bgmSoundBar.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < Mathf.Round(sfxVolume * 10); i++)
        {
            sfxSoundBar.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void PlaySound(string key)
    {
        sounds[key].Play();
    }
    public void PlaySound(string key, float minPitch, float maxPitch)
    {
        sounds[key].pitch = Random.Range(minPitch, maxPitch);
        sounds[key].Play();
    }
    public void StopSound(string key)
    {
        sounds[key].Stop();
    }
    public AudioSource GetSound(string key)
    {
        return sounds[key];
    }
    public IEnumerator FadeAudio(string key, float duration, float targetVolume)
    {
        if (sounds[key].volume == targetVolume) { yield break; }
        float currentTime = 0;
        float start = sounds[key].volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            sounds[key].volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (sounds[key].volume == 0)
        {
            sounds[key].Stop();
        }
        yield break;
    }
}
