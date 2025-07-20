/*
 * Author: Muhammad Farhan
 * Date: 19/5/2025
 * Description: Script for handling the end of levels
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    public int nextLevelIndex; // E.g., 2 if going from Level 1 to Level 2. If set to 0, triggers credits
    public GameObject creditsPanel;

    public float creditsDuration = 10f; // Duration to show credits before returning to Main Menu

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (nextLevelIndex == 0 && creditsPanel != null)
            {
                // Final level — show credits instead of next level
                creditsPanel.SetActive(true);

                // Pause gameplay
                Time.timeScale = 0f;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                StartCoroutine(ReturnToMainMenuAfterCredits());
            }

            else
            {
                int currentUnlocked = PlayerPrefs.GetInt("LastUnlockedLevel", 1);

                if (nextLevelIndex > currentUnlocked)
                {
                    PlayerPrefs.SetInt("LastUnlockedLevel", nextLevelIndex);
                }

                // Clear LastRoom so the next level spawns at its initial point
                PlayerPrefs.SetInt("LastRoom", 0);
                PlayerPrefs.SetInt("IsNewGame", 0); // Optional but safe

                //SceneManager.LoadScene("Level " + nextLevelIndex);
                LoadingManager.Instance.LoadScene("Level " +  nextLevelIndex);
            }
        }
    }

    private IEnumerator ReturnToMainMenuAfterCredits()
    {
        yield return new WaitForSecondsRealtime(creditsDuration); // Unaffected by Time.timeScale

        Time.timeScale = 1f; // Unpause
        //SceneManager.LoadScene("MainMenu");
        LoadingManager.Instance.LoadScene("MainMenu");
    }
}
