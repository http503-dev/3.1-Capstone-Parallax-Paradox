/*
 * Author: Muhammad Farhan
 * Date: 12/6/25
 * Description: Script for handling pause menu logic
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Gameplay Controllers")]
    public MonoBehaviour playerController;    
    public Superliminal superliminalScript;    

    [Header("Panels")]
    public GameObject pauseMenuPanel;       // The main pause menu panel (Resume, Settings, Exit …)
    public GameObject settingsPanel;        // The sub‐panel for adjusting volume
    public GameObject confirmationPanel;    // The confirmation popup

    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button exitToMainMenuButton;
    public Button exitToDesktopButton;

    [Header("Settings UI")]
    public Slider volumeSlider;

    [Header("Confirmation UI")]
    public TextMeshProUGUI confirmText;     // Text field to show "Are you sure?" etc.
    public Button confirmYesButton;
    public Button confirmNoButton;

    // Internal state
    private bool isPaused = false;
    private System.Action onConfirmAction;  // Action to run if the user clicks "Yes" in the confirmation popup

    private void Awake()
    {
        // Ensure all panels (except Canvas root) are off at start
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // Hook up button listeners
        resumeButton.onClick.AddListener(OnResumePressed);
        settingsButton.onClick.AddListener(OnSettingsPressed);
        exitToMainMenuButton.onClick.AddListener(OnExitToMainMenuPressed);
        exitToDesktopButton.onClick.AddListener(OnExitToDesktopPressed);

        confirmYesButton.onClick.AddListener(OnConfirmYes);
        confirmNoButton.onClick.AddListener(OnConfirmNo);

        // Hook up volume slider callback
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Load saved volume (same key as MainMenu)
        //float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //volumeSlider.value = savedVol;
        //AudioManager.Instance.SetMasterVolume(savedVol);
    }

    private void Update()
    {
        // Listen for Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        // Freeze time
        Time.timeScale = 0f;

        // Disable camera & scaling
        playerController.enabled = false;
        superliminalScript.enabled = false;

        // Show the pause menu
        pauseMenuPanel.SetActive(true);

        // Ensure settings & confirmation are hidden
        settingsPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        isPaused = false;

        // Unfreeze time
        Time.timeScale = 1f;

        // re-enable camera & scaling
        playerController.enabled = true;
        superliminalScript.enabled = true;

        // Hide all panels
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnResumePressed()
    {
        ResumeGame();
    }

    private void OnSettingsPressed()
    {
        // Open the Settings sub‐panel (inside pause menu)
        settingsPanel.SetActive(true);
        confirmationPanel.SetActive(false);
    }

    private void OnExitToMainMenuPressed()
    {
        // Show a confirmation prompt
        ShowConfirmation("Are you sure you want to exit to Main Menu?" + "\n" + "Any unsaved progress will be lost.", DoExitToMainMenu);
    }

    private void OnExitToDesktopPressed()
    {
        // Show a confirmation prompt
        ShowConfirmation("Are you sure you want to quit the game?" + "\n" + "Any unsaved progress will be lost.", DoExitToDesktop);
    }

    private void ShowConfirmation(string message, System.Action onConfirm)
    {
        // Hide Settings (if open) and ensure the pause menu is still visible behind it
        settingsPanel.SetActive(false);

        confirmText.text = message;
        confirmationPanel.SetActive(true);
        onConfirmAction = onConfirm;
    }

    private void OnConfirmYes()
    {
        confirmationPanel.SetActive(false);
        onConfirmAction?.Invoke();
        onConfirmAction = null;
    }

    private void OnConfirmNo()
    {
        confirmationPanel.SetActive(false);
        onConfirmAction = null;

        // Return to pause menu (still paused)
        settingsPanel.SetActive(false);
    }

    private void DoExitToMainMenu()
    {
        // Resume time before switching scenes
        Time.timeScale = 1f;

        // Load your main menu scene (make sure this name matches exactly)
        SceneManager.LoadScene("MainMenu");
    }

    private void DoExitToDesktop()
    {
        // Resume time before quitting (though Application.Quit won't resume time, it's good practice)
        Time.timeScale = 1f;
        Application.Quit();
    }

    private void OnVolumeChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
