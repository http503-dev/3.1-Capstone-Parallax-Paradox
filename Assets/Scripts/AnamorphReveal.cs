/*
 * Author: Muhammad Farhan
 * Date: 13/5/25
 * Description: Script for revealing 3d object after 2d image lines up
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reveals a 3D object when the player's camera is aligned with a specified forward direction
/// and within a given activation distance from the object.
/// Can optionally play a sound effect upon reveal.
/// </summary>
public class AnamorphReveal : MonoBehaviour
{
    /// <summary>
    /// The player's camera used to determine position and viewing direction.
    /// </summary>
    public Transform playerCamera;

    /// <summary>
    /// The object to reveal when the alignment and distance conditions are met. This should be set inactive initially.
    /// </summary>
    public GameObject objectToReveal;

    /// <summary>
    /// The maximum distance from the object within which it can be revealed.
    /// </summary>
    public float activationDistance = 0.5f;

    /// <summary>
    /// The maximum allowable angle difference (in degrees) between the player's forward direction
    /// and the required forward direction for the object to be revealed.
    /// </summary>
    public float maxAngleDifference = 5f; // in degrees
    public Vector3 requiredForward = Vector3.forward; // override in inspector

    /// <summary>
    /// Optional audio clip to play when the object is revealed.
    /// </summary>
    [SerializeField] private AudioClip pickupSFX;

    /// <summary>
    /// Tracks whether the object has already been revealed.
    /// </summary>
    private bool revealed = false;

    /// <summary>
    /// Checks each frame if the player is within the activation distance 
    /// and correctly aligned to reveal the object. Plays a sound if specified.
    /// </summary>
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
