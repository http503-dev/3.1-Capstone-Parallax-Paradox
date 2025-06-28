/*
 * Author: Muhammad Farhan
 * Date: 13/5/25
 * Description: Script for updating camera position in real time
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnamorphicProjectorUpdater : MonoBehaviour
{
    [Header("Camera that defines the illusion viewpoint")]
    public Camera illusionCamera;

    [Header("Material using the AnamorphicProjector shader")]
    public Material projectorMaterial;

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
