/*
 * Author: Muhammad Farhan
 * Date: 12/6/25
 * Description: Script to handle crosshair behaviour when looking at scalable objects
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScaler : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [Tooltip("The UI Image component of the crosshair.")]
    public RectTransform crosshairRect;

    [Tooltip("Scale when not over a targetable object.")]
    public Vector3 normalScale = Vector3.one * 1f;

    [Tooltip("Scale when hovering over a 'superliminal' object.")]
    public Vector3 hoverScale = Vector3.one * 2f;

    [Tooltip("Speed at which crosshair scales (optional). Set to 0 for instant switch).")]
    [Range(0f, 20f)]
    public float scaleLerpSpeed = 10f;

    [Header("Detection Settings")]
    [Tooltip("Maximum distance to raycast from the camera.")]
    public float maxRayDistance = 100f;

    // LayerMask for objects on the "targetable" layer
    private int targetableLayerMask;

    // Cached Camera reference
    private Camera mainCam;

    void Awake()
    {
        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Cache main camera
        mainCam = Camera.main;

        // Precompute the layer mask for the "targetable" layer
        // (Only that single layer will be tested by the raycast)
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
            if (hitInfo.collider.CompareTag("Superliminal"))
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
