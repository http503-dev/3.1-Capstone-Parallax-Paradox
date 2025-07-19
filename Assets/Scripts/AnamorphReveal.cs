/*
 * Author: Muhammad Farhan
 * Date: 13/5/25
 * Description: Script for revealing 3d object after 2d image lines up
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnamorphReveal : MonoBehaviour
{
    public Transform playerCamera;     // your player's camera
    public GameObject objectToReveal;  // object to show (set inactive initially)
    public float activationDistance = 0.5f;
    public float maxAngleDifference = 5f; // in degrees
    public Vector3 requiredForward = Vector3.forward; // override in inspector

    [SerializeField] private AudioClip pickupSFX;

    private bool revealed = false;

    void Update()
    {
        if (revealed) return;

        float distance = Vector3.Distance(playerCamera.position, transform.position);
        if (distance > activationDistance) return;

        // Check direction match
        Vector3 playerForward = playerCamera.forward.normalized;
        Vector3 illusionForward = requiredForward.normalized;

        float angle = Vector3.Angle(playerForward, illusionForward);

        if (angle <= maxAngleDifference)
        {
            objectToReveal.SetActive(true);
            revealed = true;

            // Play pickup sound effect
            if (pickupSFX != null)
            {
                AudioManager.Instance.PlaySFX(pickupSFX, transform.position);
            }

            Debug.Log("Illusion revealed!");
        }
    }
}
