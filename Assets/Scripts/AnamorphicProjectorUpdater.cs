/*
 * Author: Muhammad Farhan
 * Date: 13/5/25
 * Description: Script for updating camera position in real time
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates the material properties of an anamorphic projector shader 
/// based on the position, direction, and settings of a specified camera.
/// </summary>
public class AnamorphicProjectorUpdater : MonoBehaviour
{
    /// <summary>
    /// Camera that defines the viewpoint for the illusion.
    /// </summary>
    [Header("Camera that defines the illusion viewpoint")]
    public Camera illusionCamera;

    /// <summary>
    /// Material that uses the AnamorphicProjector shader.
    /// </summary>
    [Header("Material using the AnamorphicProjector shader")]
    public Material projectorMaterial;

    /// <summary>
    /// Called once per frame. Updates the shader's projection parameters
    /// to match the camera's position, direction, field of view, and aspect ratio.
    /// </summary>
    private void Update()
    {
        if (illusionCamera == null || projectorMaterial == null)
            return;

        Vector3 origin = illusionCamera.transform.position;
        Vector3 dir = illusionCamera.transform.forward;
        float fov = illusionCamera.fieldOfView;
        float aspect = illusionCamera.aspect;

        projectorMaterial.SetVector("_ProjectionOrigin", new Vector4(origin.x, origin.y, origin.z, 1f));
        projectorMaterial.SetVector("_ProjectionDir", new Vector4(dir.x, dir.y, dir.z, 0f));
        projectorMaterial.SetFloat("_FOV", fov);
        projectorMaterial.SetFloat("_Aspect", aspect);
    }
}
