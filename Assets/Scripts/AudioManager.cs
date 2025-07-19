/*
 * Author: Muhammad Farhan
 * Date: 19/5/2025
 * Description: Script for audion manager singleton
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;

    [Header("Mixer & Exposed Parameters")]
    public AudioMixer mainMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip level1BGM;
    public AudioClip level2BGM;
    public AudioClip level3BGM;
    public AudioClip level4BGM;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
        Invoke(nameof(TryBindSliders), 0.1f); // Slight delay to allow UI to finish loading
    }

    public void PlayBGMForScene(string sceneName)
    {
        AudioClip clipToPlay = sceneName switch
        {
            "MainMenu" => mainMenuBGM,
            "Level 1" => level1BGM,
            "Level 2" => level2BGM,
            "Level 3" => level3BGM,
            "Level 4" => level4BGM,
            _ => null
        };

        if (clipToPlay != null && bgmSource.clip != clipToPlay)
        {
            bgmSource.clip = clipToPlay;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, PlayerPrefs.GetFloat("SFXVolume", 1f));
    }


    public void SetMasterVolume(float value)
    {
        mainMixer.SetFloat(masterVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat(musicVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat(sfxVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void LoadVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    public void TryBindSliders()
    {
        Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();

        Slider master = System.Array.Find(allSliders, s => s.name == "Master Volume Slider");
        Slider music = System.Array.Find(allSliders, s => s.name == "BGM Volume Slider");
        Slider sfx = System.Array.Find(allSliders, s => s.name == "SFX Volume Slider");

        if (master && music && sfx)
        {
            Debug.Log("Sliders found and bound via Resources.FindObjectsOfTypeAll");

            SyncVolumeSliders(master, music, sfx);

            master.onValueChanged.RemoveAllListeners(); // avoid duplicates
            music.onValueChanged.RemoveAllListeners();
            sfx.onValueChanged.RemoveAllListeners();

            master.onValueChanged.AddListener(SetMasterFromSlider);
            music.onValueChanged.AddListener(SetMusicFromSlider);
            sfx.onValueChanged.AddListener(SetSFXFromSlider);
        }
        else
        {
            Debug.LogWarning("One or more sliders not found. Check names or ensure they exist in the scene.");
        }
    }

    public void SyncVolumeSliders(Slider master, Slider music, Slider sfx)
    {
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetMasterFromSlider(float value) => SetMasterVolume(value);
    public void SetMusicFromSlider(float value) => SetMusicVolume(value);
    public void SetSFXFromSlider(float value) => SetSFXVolume(value);
}
