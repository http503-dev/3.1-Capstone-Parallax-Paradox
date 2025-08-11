/*
 * Author: Muhammad Farhan
 * Date: 27/7/2025
 * Description: Script for handling controls tutorial at the start
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the display of a controls tutorial UI when the player enters a trigger zone.
/// The UI is shown when the player is inside the zone and hidden when they exit.
/// </summary>
public class ControlsTutorial : MonoBehaviour
{
    /// <summary>
    /// The UI GameObject that displays the control scheme.
    /// </summary>
    [Header("UI")]
    public GameObject controlsTutorialUI; // The UI to display the control scheme

    /// <summary>
    /// Initializes the tutorial by ensuring the UI is hidden at the start.
    /// </summary>
    private void Start()
    {
        controlsTutorialUI.SetActive(false);
    }

    /// <summary>
    /// Continuously shows the tutorial UI while the player stays in the trigger zone.
    /// </summary>
    /// <param name="other">The collider of the object staying in the trigger zone.</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controlsTutorialUI.SetActive(true);
        }
    }

    /// <summary>
    /// Shows the tutorial UI when the player first enters the trigger zone.
    /// </summary>
    /// <param name="other">The collider of the object entering the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controlsTutorialUI.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the tutorial UI when the player exits the trigger zone.
    /// </summary>
    /// <param name="other">The collider of the object exiting the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controlsTutorialUI.SetActive(false);
        }
    }
}
