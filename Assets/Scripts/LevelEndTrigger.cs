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
    public int nextLevelIndex; // E.g., 2 if going from Level1 to Level2

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int currentUnlocked = PlayerPrefs.GetInt("LastUnlockedLevel", 1);

            if (nextLevelIndex > currentUnlocked)
            {
                PlayerPrefs.SetInt("LastUnlockedLevel", nextLevelIndex);
            }

            SceneManager.LoadScene("Level " + nextLevelIndex);
        }
    }
}
