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

/// <summary>
/// Handles all main menu interactions including continuing games, starting new games,
/// accessing settings, level selection, room selection, and confirmation prompts.
/// Also manages dynamic background image rotation.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Primary navigation buttons
    /// </summary>
    [Header("Main Buttons")]
    public Button continueButton;
    public Button levelSelectButton;

    /// <summary>
    /// Text labels for main menu buttons
    /// </summary>
    [Header("Text References")]
    public TextMeshProUGUI continueButtonText;
    public TextMeshProUGUI levelSelectButtonText;

    /// <summary>
    /// Main menu & level select panels.
    /// </summary>
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    /// <summary>
    /// Level select UI.
    /// </summary>
    [Header("Level Select Buttons")]
    public Button[] levelButtons;
    public TextMeshProUGUI[] levelButtonTexts;
    [Header("Level Button Images")]
    public Image[] levelButtonImages;

    /// <summary>
    /// Room select UI.
    /// </summary>
    [Header("Room Select")]
    public GameObject roomSelectPanel;
    public Button[] roomButtons; // Room 1–5
    public TextMeshProUGUI[] roomButtonTexts;
    private int currentRoomLevel = 1;

    /// <summary>
    /// Settings, confirmation (with all the UI elements) & how to play panels.
    /// </summary>
    [Header("Settings")]
    public GameObject settingsPanel;
    [Header("How To Play")]
    public GameObject howToPlayPanel;
    [Header("Confirmation Panel")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText;
    private System.Action confirmAction; // Stores the action to perform if confirmed

    /// <summary>
    /// Background image cycling.
    /// </summary>
    [Header("Background Images")]
    public Image backgroundPanel;  // Image component in the main menu to change randomly
    public Sprite[] backgroundImages; // Array of sprites (images) to rotate through
    public float imageSwitchInterval = 5f;  // Time interval to switch the image

    /// <summary>
    /// Initializes the main menu, sets button states based on save data, and starts background image rotation.
    /// </summary>
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
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

        // Start the background image switching
        StartCoroutine(SwitchBackgroundImage());
    }

    /// <summary>
    /// Rotates the background image at a fixed interval.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwitchBackgroundImage()
    {
        while (true)
        {
            // Randomly choose a new image from the array
            int randomIndex = Random.Range(0, backgroundImages.Length);
            backgroundPanel.sprite = backgroundImages[randomIndex];

            // Wait for the specified interval before changing the image again
            yield return new WaitForSeconds(imageSwitchInterval);
        }
    }

    /// <summary>
    /// Continues from the last saved progress.
    /// </summary>
    public void OnContinue()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            int lastLevel = PlayerPrefs.GetInt("LastUnlockedLevel", 1);
            string levelKey = $"HighestRoomReached_Level {lastLevel}";
            int highestRoom = PlayerPrefs.GetInt(levelKey, 1); // default to Room 1

            // Force override LastRoom so RoomManager spawns player at the highest room
            PlayerPrefs.SetInt("LastRoom", highestRoom);

            LoadingManager.Instance.LoadScene("Level " + lastLevel);
        }
    }

    /// <summary>
    /// Starts a new game and resets relevant save data.
    /// </summary>
    public void OnNewGame()
    {
        // Clear all previous room progress (optional: loop through known levels)
        for (int i = 1; i <= 5; i++)
        {
            PlayerPrefs.DeleteKey($"HighestRoomReached_Level {i}");
        }

        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetInt("IsNewGame", 1); // Flag so we know it's a fresh start
        PlayerPrefs.SetInt("LastUnlockedLevel", 1);
        PlayerPrefs.SetInt("LastRoom", 0);
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
        LoadingManager.Instance.LoadScene("Level " + levelIndex);
    }

    /// <summary>
    /// Configures level selection buttons based on unlocked levels.
    /// </summary>
    /// <param name="unlockedLevel"></param>
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

    /// <summary>
    /// Opens room selection for the chosen level.
    /// </summary>
    /// <param name="levelIndex"></param>
    private void OpenRoomSelect(int levelIndex)
    {
        currentRoomLevel = levelIndex;
        levelSelectPanel.SetActive(false);
        roomSelectPanel.SetActive(true);

        string levelName = $"Level {levelIndex}";
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

        string sceneName = SceneManager.GetActiveScene().name;
        LoadingManager.Instance.LoadScene(sceneName);
    }

    public void OnOpenHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void OnCloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Shows a confirmation dialog with a custom message and action.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="onConfirm"></param>
    public void ShowConfirmation(string message, System.Action onConfirm)
    {
        confirmationText.text = message;
        confirmationPanel.SetActive(true);
        confirmAction = onConfirm;
    }

    public void OnConfirm()
    {
        confirmationPanel.SetActive(false);
        confirmAction?.Invoke();
        confirmAction = null;
    }

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
