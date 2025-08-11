/*
 * Author: Muhammad Farhan
 * Date: 25/4/25
 * Description: Script for teleporting the player
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleports the player to a predefined target position when they enter the trigger zone.
/// </summary>
public class Teleport : MonoBehaviour
{
    /// <summary>
    /// The transform position to teleport to.
    /// </summary>
    public Transform teleportTarget;

    /// <summary>
    /// The player game object.
    /// </summary>
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Teleport");
        player.transform.position = teleportTarget.transform.position;
    }
}
