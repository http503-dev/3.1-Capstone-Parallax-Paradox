/*
 * Author: Muhammad Farhan
 * Date: 25/4/25
 * Description: Script for teleporting the player
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Teleport");
        player.transform.position = teleportTarget.transform.position;
    }
}
