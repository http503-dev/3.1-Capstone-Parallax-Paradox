/*
 * Author: Muhammad Farhan
 * Date: 27/7/2025
 * Description: Script for handling controls tutorial at the start
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTutorial : MonoBehaviour
{
    [Header("UI")]
    public GameObject controlsTutorialUI; // The UI to display the control scheme

    private void Start()
    {
        // Ensure the tutorial UI is hidden at the start
        controlsTutorialUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            // Show the controls tutorial UI
            controlsTutorialUI.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            // Show the controls tutorial UI
            controlsTutorialUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            // Hide the controls tutorial UI
            controlsTutorialUI.SetActive(false);
        }
    }
}
