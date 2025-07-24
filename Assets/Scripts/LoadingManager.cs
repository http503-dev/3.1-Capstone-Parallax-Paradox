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

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI")]
    public GameObject loadingScreen;
    public Slider progressBar;

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

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;
        float minDisplayTime = 1.5f;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            timer += Time.deltaTime;
            yield return null;
        }

        // Progress is at 90% - scene is ready but not yet activated
        if (progressBar != null)
            progressBar.value = 1f;

        // Wait remaining time if scene loaded too fast
        while (timer < minDisplayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Let Unity switch to the new scene
        operation.allowSceneActivation = true;

        // Wait one frame to let the scene fully activate
        yield return null;

        loadingScreen.SetActive(false); // Now it's safe to hide the screen
    }
}
