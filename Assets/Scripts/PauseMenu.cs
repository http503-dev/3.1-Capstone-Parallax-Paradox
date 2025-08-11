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

/// <summary>
/// Handles game pausing, resuming, settings, and confirmation prompts.
/// Disables player controls when paused and manages related UI panels.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// The player movement/look controller & Superliminal scaling script to disable when paused.
    /// </summary>
    [Header("Gameplay Controllers")]
    public MonoBehaviour playerController;    
    public Superliminal superliminalScript;    

    /// <summary>
    /// Panels used in the pause menu.
    /// </summary>
    [Header("Panels")]
    public GameObject pauseMenuPanel;      
    public GameObject settingsPanel;        
    public GameObject confirmationPanel;    

    /// <summary>
    /// Buttons to access different functions/panels.
    /// </summary>
    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button resetRoomButton;  
    public Button exitToMainMenuButton;
    public Button exitToDesktopButton;

    /// <summary>
    /// Confirmation UI elements.
    /// </summary>
    [Header("Confirmation UI")]
    public TextMeshProUGUI confirmText;     
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
        resetRoomButton.onClick.AddListener(OnResetRoomPressed);
        exitToMainMenuButton.onClick.AddListener(OnExitToMainMenuPressed);
        exitToDesktopButton.onClick.AddListener(OnExitToDesktopPressed);

        confirmYesButton.onClick.AddListener(OnConfirmYes);
        confirmNoButton.onClick.AddListener(OnConfirmNo);
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

    /// <summary>
    /// Pauses the game, disables controls, and shows the pause menu.
    /// </summary>
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

    /// <summary>
    /// Resumes gameplay, re-enables controls, and hides menus.
    /// </summary>
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

    private void OnResetRoomPressed()
    {
        ShowConfirmation("Are you sure you want to reset this room?\nOnly do this if you're soft locked.", DoResetRoom);
    }

    private void DoResetRoom()
    {
        Time.timeScale = 1f; // Unpause
        string currentScene = SceneManager.GetActiveScene().name;
        LoadingManager.Instance.LoadScene(currentScene);
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

    /// <summary>
    /// Displays a confirmation dialog with the provided message and action.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="onConfirm"></param>
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

        // Load your main menu scene
        LoadingManager.Instance.LoadScene("MainMenu");
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
