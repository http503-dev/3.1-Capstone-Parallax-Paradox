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

/// <summary>
/// Manages all audio in the game, including background music (BGM) and sound effects (SFX).
/// Provides volume control, scene-based BGM switching, and UI slider integration.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager Instance;

    /// <summary>
    /// The audio source used to play background music.
    /// </summary>
    [Header("Audio Sources")]
    public AudioSource bgmSource;

    /// <summary>
    /// Main audio mixer controlling all audio levels and parameter names for the mixer.
    /// </summary>
    [Header("Mixer & Exposed Parameters")]
    public AudioMixer mainMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    /// <summary>
    /// Background music for the different scenes.
    /// </summary>
    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip level1BGM;
    public AudioClip level2BGM;
    public AudioClip level3BGM;
    public AudioClip level4BGM;

    /// <summary>
    /// Ensures the singleton instance is initialized and persists across scenes.
    /// Loads saved volume settings.
    /// </summary>
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

    /// <summary>
    /// Subscribes to scene load events.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from scene load events.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Called when a new scene is loaded. Plays appropriate BGM and attempts to bind sliders.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
        Invoke(nameof(TryBindSliders), 0.1f); // Slight delay to allow UI to finish loading
    }

    /// <summary>
    /// Plays the background music associated with the given scene name.
    /// </summary>
    /// <param name="sceneName">The name of the current scene.</param>
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

    /// <summary>
    /// Plays a one-shot sound effect at the specified position with the current SFX volume.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The world position to play the sound at.</param>
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    /// <summary>
    /// Sets the master volume and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetMasterVolume(float value)
    {
        mainMixer.SetFloat(masterVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    /// <summary>
    /// Sets the music volume and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat(musicVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    /// <summary>
    /// Sets the SFX volume and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat(sfxVolumeParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    /// <summary>
    /// Loads saved volume levels from PlayerPrefs and applies them.
    /// </summary>
    private void LoadVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    /// <summary>
    /// Attempts to bind volume sliders in the scene to the appropriate volume control methods. Searches for sliders by name and attaches listeners.
    /// </summary>
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

    /// <summary>
    /// Synchronizes the UI sliders' values with the saved volume settings.
    /// </summary>
    /// <param name="master">Master slider.</param>
    /// <param name="music">Music slider.</param>
    /// <param name="sfx">SFX slider.</param>
    public void SyncVolumeSliders(Slider master, Slider music, Slider sfx)
    {
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    /// <summary>
    /// Sets the master volume from a slider's value.
    /// </summary>
    /// <param name="value">The volume value.</param>
    public void SetMasterFromSlider(float value) => SetMasterVolume(value);

    /// <summary>
    /// Sets the music volume from a slider's value.
    /// </summary>
    /// <param name="value">The volume value.</param>
    public void SetMusicFromSlider(float value) => SetMusicVolume(value);

    /// <summary>
    /// Sets the SFX volume from a slider's value.
    /// </summary>
    /// <param name="value">The volume value.</param>
    public void SetSFXFromSlider(float value) => SetSFXVolume(value);
}
