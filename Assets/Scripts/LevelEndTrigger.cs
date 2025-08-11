/*
 * Author: Muhammad Farhan
 * Date: 19/5/2025
 * Description: Script for handling the end of levels
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the logic for when the player reaches the end of a level.
/// Can either load the next level or trigger the credits screen for the final level.
/// </summary>
public class LevelEndTrigger : MonoBehaviour
{
    /// <summary>
    /// The index of the next level to load.
    /// </summary>
    public int nextLevelIndex; // E.g., 2 if going from Level 1 to Level 2. If set to 0, triggers credits

    /// <summary>
    /// The UI panel displaying the credits.
    /// </summary>
    public GameObject creditsPanel;

    /// <summary>
    /// The duration (in seconds) to display credits before returning to the main menu.
    /// </summary>
    public float creditsDuration = 10f;

    /// <summary>
    /// Triggered when another collider enters the trigger zone.
    /// If the player enters, either shows credits (final level) or loads the next level.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Final level — show credits instead of loading another level
            if (nextLevelIndex == 0 && creditsPanel != null)
            {
                creditsPanel.SetActive(true);

                // Pause gameplay
                Time.timeScale = 0f;

                // Unlock cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                StartCoroutine(ReturnToMainMenuAfterCredits());
            }

            else
            {
                int currentUnlocked = PlayerPrefs.GetInt("LastUnlockedLevel", 1);

                // Unlock next level if not already unlocked
                if (nextLevelIndex > currentUnlocked)
                {
                    PlayerPrefs.SetInt("LastUnlockedLevel", nextLevelIndex);
                }

                // Clear LastRoom so the next level spawns at its initial point
                PlayerPrefs.SetInt("LastRoom", 0);
                PlayerPrefs.SetInt("IsNewGame", 0);

                // Load next level
                LoadingManager.Instance.LoadScene("Level " +  nextLevelIndex);
            }
        }
    }

    /// <summary>
    /// Waits for the credits to finish, then returns to the main menu.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnToMainMenuAfterCredits()
    {
        yield return new WaitForSecondsRealtime(creditsDuration); // Unaffected by Time.timeScale

        Time.timeScale = 1f; // Unpause
        LoadingManager.Instance.LoadScene("MainMenu");
    }
}
