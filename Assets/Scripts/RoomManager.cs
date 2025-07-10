/*
 * Author: Muhammad Farhan
 * Date: 27/6/25
 * Description: Handles saving player's room position and respawning based on saved room
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [Header("Room Spawn Points")]
    public Transform[] roomSpawnPoints; // Set in inspector: Room 1 -> index 0, Room 2 -> index 1, etc.

    [Header("New Game Spawn")]
    public Transform newGameSpawnPoint;

    [Header("Player Reference")]
    public Transform player; 

    private void Start()
    {
        int isNewGame = PlayerPrefs.GetInt("IsNewGame", 0);
        int lastRoom = PlayerPrefs.GetInt("LastRoom", 0); // 0 = start location

        if (isNewGame == 1 || lastRoom == 0)
        {
            PlayerPrefs.SetInt("IsNewGame", 0); // Reset flag
            Debug.Log("Spawning at NEW GAME spawn point.");
            player.position = newGameSpawnPoint.position;
            player.rotation = newGameSpawnPoint.rotation;
            return;
        }

        // Spawn at saved room
        int index = Mathf.Clamp(lastRoom - 1, 0, roomSpawnPoints.Length - 1);
        Debug.Log($"Spawning at ROOM {lastRoom} spawn point.");
        player.position = roomSpawnPoints[index].position;
        player.rotation = roomSpawnPoints[index].rotation;
    }

    public void SaveRoomProgress(int roomIndex)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string key = $"HighestRoomReached_{currentScene}";
        int previousRoom = PlayerPrefs.GetInt(key, 0);

        // Always save where the player was last
        PlayerPrefs.SetInt("LastRoom", roomIndex);
        PlayerPrefs.SetInt("HasSave", 1);

        Debug.Log($"Room {roomIndex} entered. Previous highest: {previousRoom}");

        if (roomIndex > previousRoom)
        {
            PlayerPrefs.SetInt(key, roomIndex);
            Debug.Log($"New highest room for {currentScene}: {roomIndex}");
        }
    }

    // Optional debug to clear room save (e.g., for testing)
    [ContextMenu("Clear Room Save")]
    public void ClearRoomSave()
    {
        PlayerPrefs.DeleteKey("LastRoom");
    }
}
