/*
 * Author: Muhammad Farhan
 * Date: 12/6/25
 * Description: Script to handle crosshair behaviour when looking at scalable objects
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the scaling of a crosshair UI element when the player looks at specific targetable objects.
/// The crosshair scales up when hovering over objects with the "Superliminal" or "SuperliminalTutorial" tags.
/// </summary>
public class CrosshairScaler : MonoBehaviour
{
    /// <summary>
    /// The RectTransform component of the crosshair UI element.
    /// </summary>
    [Header("Crosshair Settings")]
    [Tooltip("The UI Image component of the crosshair.")]
    public RectTransform crosshairRect;

    /// <summary>
    /// The scale of the crosshair when not pointing at a valid target.
    /// </summary>
    [Tooltip("Scale when not over a targetable object.")]
    public Vector3 normalScale = Vector3.one * 1f;

    /// <summary>
    /// The scale of the crosshair when pointing at a valid target.
    /// </summary>
    [Tooltip("Scale when hovering over a 'superliminal' object.")]
    public Vector3 hoverScale = Vector3.one * 5f;

    /// <summary>
    /// The interpolation speed for scaling the crosshair.
    /// </summary>
    [Tooltip("Speed at which crosshair scales (optional). Set to 0 for instant switch).")]
    [Range(0f, 20f)]
    public float scaleLerpSpeed = 6f;

    /// <summary>
    /// The maximum raycast distance to detect targetable objects.
    /// </summary>
    [Header("Detection Settings")]
    [Tooltip("Maximum distance to raycast from the camera.")]
    public float maxRayDistance = 40f;

    /// <summary>
    /// Layer mask for objects on the "Targetable" layer.
    /// </summary>
    private int targetableLayerMask;

    /// <summary>
    /// Cached reference to the main camera.
    /// </summary>
    private Camera mainCam;

    /// <summary>
    /// Initializes variables, hides the cursor, and sets the initial crosshair scale.
    /// </summary>
    void Awake()
    {
        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Cache main camera
        mainCam = Camera.main;

        // Precompute the layer mask for the "targetable" layer
        targetableLayerMask = 1 << LayerMask.NameToLayer("Targetable");

        // If crosshairRect isn't assigned, try to grab the RectTransform on this GameObject
        if (crosshairRect == null)
        {
            crosshairRect = GetComponent<RectTransform>();
            if (crosshairRect == null)
                Debug.LogError("CrosshairScaler: No RectTransform found. Assign the crosshair UI Image manually.");
        }

        // Initialize to normal scale
        crosshairRect.localScale = normalScale;
    }

    /// <summary>
    /// Performs a raycast from the center of the screen and adjusts the crosshair scale 
    /// based on whether a valid target is detected.
    /// </summary>
    void Update()
    {
        // 1. Perform a raycast from the center of the screen
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, maxRayDistance, targetableLayerMask);

        // 2. Check if it hit an object with tag "superliminal"
        bool overValid = false;
        if (hit)
        {
            // See if the hit object's tag is exactly "superliminal"
            if (hitInfo.collider.CompareTag("Superliminal") || hitInfo.collider.CompareTag("SuperliminalTutorial"))
            {
                overValid = true;
            }
        }

        // 3. Determine target scale
        Vector3 targetScale = overValid ? hoverScale : normalScale;

        // 4. Smoothly lerp (or instantly switch if speed = 0)
        if (scaleLerpSpeed > 0f)
        {
            crosshairRect.localScale = Vector3.Lerp(
                crosshairRect.localScale,
                targetScale,
                Time.deltaTime * scaleLerpSpeed
            );
        }
        else
        {
            crosshairRect.localScale = targetScale;
        }
    }
}
