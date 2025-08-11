/*
 * Author: Muhammad Farhan
 * Date: 23/4/25
 * Description: Script for non-euclidean/forced perspective scaling mechanic
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a forced perspective object scaling mechanic.
/// Allows picking up, moving, and resizing objects based on camera distance.
/// </summary>
public class Superliminal : MonoBehaviour
{
    /// <summary>
    /// The currently held object, if any.
    /// </summary>
    [Header("Components")]
    public Transform target;

    /// <summary>
    /// Parameters for scaling mechanic.
    /// </summary>
    [Header("Parameter")]
    public LayerMask targetMask;   // The layer mask used to hit only potential targets with a raycast
    public LayerMask ignoreTargetMask;   // The layer mask used to ignore the player and target objects while raycasting
    public float offsetFactor;   // The offset amount for positioning the object so it doesn't clip into walls

    [SerializeField] private AudioClip pickupSFX;

    /// <summary>
    /// Scaling and distance info.
    /// </summary>
    float originalDistance;   // The original distance between the player camera and the target
    float originalScale;   // The original scale of the target objects prior to being resized
    Vector3 targetScale;   // The scale we want our object to be set to each frame

    private Rigidbody targetRigidbody;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        ResizeTarget();
    }

    /// <summary>
    /// Handles mouse input for picking up and dropping scalable objects.
    /// </summary>
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (target == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetMask))   // Fire a raycast with the layer mask that only hits potential targets
                {
                    // Check if anything between player and object is a force field
                    RaycastHit[] obstructionHits = Physics.RaycastAll(transform.position, transform.forward, hit.distance, ~0); // ~0 = all layers
                    foreach (var obstruction in obstructionHits)
                    {
                        if (obstruction.collider.CompareTag("ForceField"))
                        {
                            return; // There's a force field in the way, can't pick this up
                        }
                    }

                    target = hit.transform;
                    targetRigidbody = target.GetComponent<Rigidbody>();
                    if (targetRigidbody != null)
                    {
                        target.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    originalDistance = Vector3.Distance(transform.position, target.position);   // Calculate the distance between the camera and the object
                    originalScale = target.localScale.x;   // Save the original scale of the object
                    targetScale = target.localScale;   // Set target scale to be the same as the original

                    // Play pickup sound effect
                    if (pickupSFX != null)
                    {
                        AudioManager.Instance.PlaySFX(pickupSFX, target.position);
                    }
                }
            }

            else
            {
                if (targetRigidbody != null)
                {
                    targetRigidbody.isKinematic = false;
                }
                target = null;
            }
        }
    }

    /// <summary>
    /// Adjusts the held object's position and scale based on the camera's distance.
    /// </summary>
    void ResizeTarget()
    {
        if (target == null)
        {
            return;
        }

        RaycastHit hit;   // Cast a ray forward from the camera position 
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ignoreTargetMask))   // ignore the layer that is used to acquire targets so we don't hit the attached target with our ray
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PortalView"))
            {
                return;
            }
            // If the hit object is a force field, drop the object
            if (hit.collider.CompareTag("ForceField"))
            {
                if (targetRigidbody != null)
                    targetRigidbody.isKinematic = false;

                target = null;
                return;
            }

            target.position = hit.point - transform.forward * offsetFactor * targetScale.x;   // Set the new position of the target by getting the hit point and moving it back a bit depending on the scale and offset factor

            float currentDistance = Vector3.Distance(transform.position, target.position);   // Calculate the current distance between the camera and the target object
            float s = currentDistance / originalDistance;   // Calculate the ratio between the current distance and the original distance
            targetScale.x = targetScale.y = targetScale.z = s;    // Set the scale Vector3 variable to be the ratio of the distances

            target.transform.localScale = targetScale * originalScale;   // Set the scale for the target objectm, multiplied by the original scale
        }
    }

    /// <summary>
    /// Forces the player to drop the held object, used when intersecting with forcefields.
    /// </summary>
    public void ForceDrop()
    {
        if (targetRigidbody != null)
            targetRigidbody.isKinematic = false;

        target = null;
    }

}
