/*
 * Author: Muhammad Farhan
 * Date: 24/7/2025
 * Description: Script for handling tutorial hints on the first interaction of the level
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    public Camera playerCamera;  // Reference to the player's camera
    public GameObject hintUI;    // Reference to the hint UI

    public float raycastDistance = 8f; // How far the raycast can detect objects
    public string[] interactableTags = { "Superliminal", "PortalProp", "AnamorphicProp" }; // Tags for interactable props

    private void Start()
    {
        // Ensure the hint UI is hidden at the start
        hintUI.SetActive(false);
    }

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)); // Cast a ray from the center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance)) // If something is hit within the ray distance
        {
            bool isInteractable = false;

            // Check if the hit object has any of the specified tags
            foreach (var tag in interactableTags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    isInteractable = true;
                    break;
                }
            }

            if (isInteractable)
            {
                // Show the hint UI when looking at an interactable object
                if (!hintUI.activeSelf)
                    hintUI.SetActive(true);
            }
            else
            {
                // Hide the hint UI when not looking at an interactable object
                if (hintUI.activeSelf)
                    hintUI.SetActive(false);
            }
        }
        else
        {
            // Hide the hint UI if the ray doesn't hit anything
            if (hintUI.activeSelf)
                hintUI.SetActive(false);
        }
    }
}
