/*
 * Author: Muhammad Farhan
 * Date: 27/6/25
 * Description: Script to handle player teleportation through portals
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles teleportation of the player and objects through linked portals,
/// ensuring correct position, rotation, and cooldown to prevent teleport loops.
/// </summary>
public class PortalTeleporter : MonoBehaviour
{
    /// <summary>
    /// The linked portal to teleport to.
    /// </summary>
    public PortalTeleporter otherTeleporter;

    /// <summary>
    /// Minimum time in seconds before the same object can teleport again.
    /// </summary>
    public float teleportCooldown = 0.2f;

    /// <summary>
    /// Tracks the last time each object teleported to prevent rapid re-teleportation.
    /// </summary>
    private Dictionary<Transform, float> lastTeleportTime = new Dictionary<Transform, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (!CanTeleport(other)) return;

        // Use the Z-position in local space to determine entry direction
        float localZ = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;
        if (localZ < 0)
        {
            Teleport(other);
        }
    }

    /// <summary>
    /// Determines if the object is allowed to teleport.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool CanTeleport(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Superliminal"))
        {
            return false;
        }

        float lastTime;
        if (lastTeleportTime.TryGetValue(other.transform, out lastTime))
        {
            if (Time.time - lastTime < teleportCooldown) return false;
        }

        return true;
    }

    /// <summary>
    /// Teleports a held object relative to the player's portal transition.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rotationDelta"></param>
    private void TeleportHeldObject(Transform target, Quaternion rotationDelta)
    {
        Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(target.position);
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        target.position = otherTeleporter.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

        target.rotation = rotationDelta * target.rotation;
    }

    /// <summary>
    /// Teleports the object through the portal, adjusting position, rotation,
    /// and handling special logic for players and carried objects.
    /// </summary>
    /// <param name="other"></param>
    private void Teleport(Collider other)
    {
        Transform obj = other.transform;

        // Prevent teleport loops
        lastTeleportTime[obj] = Time.time;
        otherTeleporter.lastTeleportTime[obj] = Time.time;

        // Special logic if it's the player
        if (other.CompareTag("Player"))
        {
            CharacterController cc = obj.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
            localPos = new Vector3(-localPos.x, localPos.y, -localPos.z); // mirror through portal
            Vector3 offset = otherTeleporter.transform.forward * -0.7f;
            obj.position = otherTeleporter.transform.localToWorldMatrix.MultiplyPoint3x4(localPos) + offset;

            Quaternion rotDiff = otherTeleporter.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
            obj.rotation = rotDiff * obj.rotation;

            if (cc != null) cc.enabled = true;

            // Teleport any carried object as well
            Superliminal superliminal = FindObjectOfType<Superliminal>();
            if (superliminal != null && superliminal.target != null)
            {
                Collider targetCol = superliminal.target.GetComponent<Collider>();
                if (targetCol != null) targetCol.enabled = false;

                TeleportHeldObject(superliminal.target, rotDiff);

                if (targetCol != null) targetCol.enabled = true;
            }
        }

        // If it's just a loose object (not the player)
        else if (other.CompareTag("Superliminal"))
        {
            Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
            localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
            obj.position = otherTeleporter.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

            Quaternion rotDiff = otherTeleporter.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
            obj.rotation = rotDiff * obj.rotation;
        }
    }
}
