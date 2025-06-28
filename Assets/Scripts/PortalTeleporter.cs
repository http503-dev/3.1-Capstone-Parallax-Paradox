using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public PortalTeleporter otherTeleporter;
    public float teleportCooldown = 0.2f; // prevent rapid-fire teleporting

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

    private bool CanTeleport(Collider other)
    {
        if (!other.CompareTag("Player")) return false;

        float lastTime;
        if (lastTeleportTime.TryGetValue(other.transform, out lastTime))
        {
            if (Time.time - lastTime < teleportCooldown) return false;
        }

        return true;
    }

    private void Teleport(Collider other)
    {
        Transform obj = other.transform;
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Position: relative to source portal, then to destination portal
        Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z); // mirror through portal
        Vector3 offset = otherTeleporter.transform.forward * -0.7f; // push player slightly forward from the portal
        obj.position = otherTeleporter.transform.localToWorldMatrix.MultiplyPoint3x4(localPos) + offset;

        // Rotation: match relative orientation
        Quaternion rotDiff = otherTeleporter.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
        obj.rotation = rotDiff * obj.rotation;

        if (cc != null) cc.enabled = true;

        // Apply cooldown to both portals for this object
        lastTeleportTime[obj] = Time.time;
        otherTeleporter.lastTeleportTime[obj] = Time.time;
    }
}
