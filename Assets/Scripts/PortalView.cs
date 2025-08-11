/*
 * Author: Muhammad Farhan
 * Date: 27/6/25
 * Description: Script to handle what the player sees through the portal
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders the view through a linked portal by positioning and orienting a camera
/// to simulate looking out from the other portal.
/// </summary>
public class PortalView : MonoBehaviour
{
    /// <summary>
    /// The linked portal whose view will be rendered.
    /// </summary>
    public PortalView otherPortal;

    /// <summary>
    /// Camera used to capture the view from the other portal.
    /// </summary>
    public Camera portalView;

    /// <summary>
    /// Shader used for rendering the portal surface.
    /// </summary>
    public Shader portalShader;

    [SerializeField] private MeshRenderer portalMesh;

    private Material portalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        otherPortal.portalView.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);

        portalMaterial = new Material(portalShader);
        portalMaterial.mainTexture = otherPortal.portalView.targetTexture;
        portalMesh.material = portalMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        bool isVisible = IsVisibleFrom(Camera.main);

        // Enable the portal camera only if this portal is visible
        portalView.enabled = isVisible;

        if (!IsVisibleFrom(Camera.main)) return;

        // Compute relative matrix from the main camera to the other portal
        Matrix4x4 camToPortal = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix;

        // Transform the camera's position and direction
        Vector3 newCamPos = camToPortal.MultiplyPoint(Camera.main.transform.position);
        Vector3 forward = camToPortal.MultiplyVector(Camera.main.transform.forward);
        Vector3 up = camToPortal.MultiplyVector(Camera.main.transform.up);
        Quaternion newCamRot = Quaternion.LookRotation(-forward, up); // flip forward to look out

        // Flip just the pitch (X) and roll (Z) to fix inverted tilt
        newCamRot = Quaternion.Euler(-newCamRot.eulerAngles.x, newCamRot.eulerAngles.y, -newCamRot.eulerAngles.z);

        // Calculate offset to move the camera slightly behind the portal plane
        Vector3 portalNormal = transform.forward;
        float desiredBackOffset = 0.05f;
        newCamPos -= portalNormal * desiredBackOffset;

        // Apply transform
        portalView.transform.SetPositionAndRotation(newCamPos, newCamRot);

        // Optional near clip fix to avoid slicing through mesh
        float nearOffset = Vector3.Dot(portalView.transform.forward, transform.position - newCamPos);
        portalView.nearClipPlane = Mathf.Max(0.01f, nearOffset);

    }

    /// <summary>
    /// Checks if the portal mesh is within the camera's frustum.
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    private bool IsVisibleFrom(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, portalMesh.bounds);
    }

}
