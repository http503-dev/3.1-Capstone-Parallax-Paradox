/*
 * Author: Muhammad Farhan
 * Date: 15/5/25
 * Description: Script for the forcefields that prevent players from bring props through different levels as well as saves which rooms have been completed
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a forcefield that blocks targetable objects from passing through
/// and tracks player progress by marking rooms as completed.
/// </summary>
public class ForceField : MonoBehaviour
{
    /// <summary>
    /// Index of the room this forcefield is associated with.
    /// </summary>
    public int roomIndex; // 1 = Room 1, 2 = Room 2, etc.

    /// <summary>
    /// Reference to the RoomManager responsible for saving room progress.
    /// </summary>
    public RoomManager manager;

    /// <summary>
    /// Triggered when another collider enters the forcefield's trigger area.
    /// Prevents targetable objects from passing through by forcing the player to drop them,
    /// and records room completion when the player passes through.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // If the object is on the "Targetable" layer, force the player to drop it
        if (other.gameObject.layer == LayerMask.NameToLayer("Targetable"))
        {
            Superliminal controller = FindObjectOfType<Superliminal>();
            if (controller.target != null && controller.target.gameObject == other.gameObject)
            {
                controller.ForceDrop();
            }
        }

        // If the player passes through, save the room's progress
        if (other.CompareTag("Player"))
        {
            Debug.Log("player passed");
            manager.SaveRoomProgress(roomIndex);
        }
    }
}
