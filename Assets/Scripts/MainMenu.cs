/*
 * Author: Muhammad Farhan
 * Date: 19/5/2025
 * Description: Script for handling main menu logic
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button continueButton;
    public Button levelSelectButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    [Header("Level Select Buttons")]
    public Button[] levelButtons;

    [Header("Settings")]
    public GameObject settingsPanel;
    public Slider masterVolumeSlider;

    [Header("Confirmation Panel")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText; // Or TMP_Text
    private System.Action confirmAction; // Stores the action to perform if confirmed

    private void Start()
    {
        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        int lastUnlockedLevel = PlayerPrefs.GetInt("LastUnlockedLevel", 0);
        bool hasSave = PlayerPrefs.GetInt("HasSave", 0) == 1;

        continueButton.interactable = hasSave;
        levelSelectButton.interactable = lastUnlockedLevel > 0;

        SetupLevelButtons(lastUnlockedLevel);
    }

    public void OnContinue()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            int lastLevel = PlayerPrefs.GetInt("LastUnlockedLevel", 1);
            SceneManager.LoadScene("Level " + lastLevel);  // Assuming scenes are named "Level1", "Level2", etc.
        }
    }

    public void OnNewGame()
    {
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetInt("LastUnlockedLevel", 1);
        SceneManager.LoadScene("Level 1");
    }

    public void OnOpenLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnCloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level " + levelIndex);
    }

    private void SetupLevelButtons(int unlockedLevel)
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i + 1) <= unlockedLevel;

            int index = i + 1; // avoid closure issue in lambda
            levelButtons[i].onClick.AddListener(() => LoadLevel(index));
        }
    }

    public void OnOpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterVolumeSlider.value = savedVolume;
        AudioManager.Instance.SetMasterVolume(savedVolume);
    }

    public void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnVolumeChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnDeleteSaveClicked()
    {
        ShowConfirmation("Are you sure you want to delete your save?", DeleteSave);
    }

    private void DeleteSave()
    {
        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.DeleteKey("LastUnlockedLevel");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Generic confirm popup
    public void ShowConfirmation(string message, System.Action onConfirm)
    {
        confirmationText.text = message;
        confirmationPanel.SetActive(true);
        confirmAction = onConfirm;
    }

    // Hook this to the Confirm button
    public void OnConfirm()
    {
        confirmationPanel.SetActive(false);
        confirmAction?.Invoke();
        confirmAction = null;
    }

    // Hook this to the Cancel button
    public void OnCancel()
    {
        confirmationPanel.SetActive(false);
        confirmAction = null;
    }

    public void OnExitClicked()
    {
        ShowConfirmation("Are you sure you want to exit the game?", ExitGame);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
