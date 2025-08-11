/*
 * Author: Muhammad Farhan
 * Date: 20/7/2025
 * Description: Script for handling the loading of levels/loading screens
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages asynchronous scene loading with a loading screen and progress bar.
/// Implements a singleton pattern to persist across scenes.
/// </summary>
public class LoadingManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Loading Manager.
    /// </summary>
    public static LoadingManager Instance;

    /// <summary>
    /// UI elements
    /// </summary>
    [Header("UI")]
    public GameObject loadingScreen;
    public Slider progressBar;

    /// <summary>
    /// Ensures there is only one instance of the Loading Manager and persists it across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initiates loading of a new scene by name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Handles asynchronous scene loading with progress bar updates and a minimum display time.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;
        float minDisplayTime = 1.5f;

        // Update the progress bar until the scene is ready
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            timer += Time.deltaTime;
            yield return null;
        }

        // Scene is ready to be activated
        if (progressBar != null)
            progressBar.value = 1f;

        // Ensure the loading screen is displayed for at least the minimum time
        while (timer < minDisplayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Activate the new scene
        operation.allowSceneActivation = true;

        // Wait one frame to ensure scene is fully loaded before hiding loading screen
        yield return null;

        // Now it's safe to hide the screen
        loadingScreen.SetActive(false); 
    }
}
