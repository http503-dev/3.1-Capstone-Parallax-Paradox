/*
 * Author: Muhammad Farhan
 * Date: 15/5/25
 * Description: Script for the forcefields that prevent players from bring props through different levels as well as saves which rooms have been completed
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public int roomIndex; // 1 = Room 1, 2 = Room 2, etc.
    public RoomManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Targetable"))
        {
            Superliminal controller = FindObjectOfType<Superliminal>();
            if (controller.target != null && controller.target.gameObject == other.gameObject)
            {
                controller.ForceDrop();
            }
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("player passed");
            manager.SaveRoomProgress(roomIndex);
        }
    }
}
