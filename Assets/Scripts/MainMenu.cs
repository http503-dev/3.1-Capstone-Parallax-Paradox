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

    [Header("Text References")]
    public TextMeshProUGUI continueButtonText;
    public TextMeshProUGUI levelSelectButtonText;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    [Header("Level Select Buttons")]
    public Button[] levelButtons;
    public TextMeshProUGUI[] levelButtonTexts;

    [Header("Level Button Images")]
    public Image[] levelButtonImages;

    [Header("Room Select")]
    public GameObject roomSelectPanel;
    public Button[] roomButtons; // Room 1–5
    public TextMeshProUGUI[] roomButtonTexts;
    private int currentRoomLevel = 1;

    [Header("Settings")]
    public GameObject settingsPanel;

    [Header("Confirmation Panel")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText; // Or TMP_Text
    private System.Action confirmAction; // Stores the action to perform if confirmed

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        roomSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        int lastUnlockedLevel = PlayerPrefs.GetInt("LastUnlockedLevel", 0);
        bool hasSave = PlayerPrefs.GetInt("HasSave", 0) == 1;

        continueButton.interactable = hasSave;
        levelSelectButton.interactable = lastUnlockedLevel > 0;

        // Gray out text only
        continueButtonText.color = hasSave ? Color.white : new Color(0.5f, 0.5f, 0.5f); // gray
        levelSelectButtonText.color = lastUnlockedLevel > 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f);

        SetupLevelButtons(lastUnlockedLevel);
    }

    public void OnContinue()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            int lastLevel = PlayerPrefs.GetInt("LastUnlockedLevel", 1);
            //SceneManager.LoadScene("Level " + lastLevel);  // Assuming scenes are named "Level1", "Level2", etc.
            LoadingManager.Instance.LoadScene("Level " + lastLevel);
        }
    }

    public void OnNewGame()
    {
        // Clear all previous room progress (optional: loop through known levels)
        for (int i = 1; i <= 5; i++)
        {
            PlayerPrefs.DeleteKey($"HighestRoomReached_Level {i}"); // Space is intentional if you're using "Level 1"
        }

        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetInt("IsNewGame", 1); // Flag so we know it's a fresh start
        PlayerPrefs.SetInt("LastUnlockedLevel", 1);
        PlayerPrefs.SetInt("LastRoom", 0);
        //SceneManager.LoadScene("Level 1");
        LoadingManager.Instance.LoadScene("Level 1");
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

    public void OnBackFromRoomSelect()
    {
        roomSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void LoadLevel(int levelIndex)
    {
        //SceneManager.LoadScene("Level " + levelIndex);
        LoadingManager.Instance.LoadScene("Level " + levelIndex);
    }

    private void SetupLevelButtons(int unlockedLevel)
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button levelButton = levelButtons[i];
            TextMeshProUGUI levelText = levelButtonTexts[i];
            Image levelImage = levelButtonImages[i];

            bool isUnlocked = levelIndex <= unlockedLevel;
            levelButton.interactable = isUnlocked;

            levelText.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f); // gray text
            levelImage.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);// Set image color (white = normal, gray = locked)

            levelButton.onClick.RemoveAllListeners();

            if (isUnlocked)
            {
                levelButton.onClick.AddListener(() => OpenRoomSelect(levelIndex));
            }
        }
    }

    private void OpenRoomSelect(int levelIndex)
    {
        currentRoomLevel = levelIndex;
        levelSelectPanel.SetActive(false);
        roomSelectPanel.SetActive(true);

        string levelName = $"Level {levelIndex}"; // Use actual scene names if needed
        string roomKey = $"HighestRoomReached_{levelName}";
        int highestRoom = PlayerPrefs.GetInt(roomKey, 0); // default 0 = no room unlocked

        for (int i = 0; i < roomButtons.Length; i++)
        {
            int roomIndex = i + 1;
            Button btn = roomButtons[i];
            TextMeshProUGUI text = roomButtonTexts[i];

            bool isUnlocked = roomIndex <= highestRoom;
            btn.interactable = isUnlocked;
            text.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            btn.onClick.RemoveAllListeners();

            if (roomIndex <= highestRoom)
            {
                btn.onClick.AddListener(() =>
                {
                    PlayerPrefs.SetInt("LastRoom", roomIndex);
                    //SceneManager.LoadScene(levelName);
                    LoadingManager.Instance.LoadScene(levelName);
                });
            }
        }
    }

    public void OnOpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnDeleteSaveClicked()
    {
        ShowConfirmation("Are you sure you want to delete your save?", DeleteSave);
    }

    private void DeleteSave()
    {
        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.DeleteKey("LastUnlockedLevel");
        PlayerPrefs.DeleteKey("LastRoom");

        // Clear room progress for all levels if needed (e.g. Level1 to Level5)
        for (int i = 1; i <= 5; i++)
        {
            PlayerPrefs.DeleteKey($"HighestRoomReached_Level{i}");
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        string sceneName = SceneManager.GetActiveScene().name;
        LoadingManager.Instance.LoadScene(sceneName);
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
