using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalView : MonoBehaviour
{
    public PortalView otherPortal;
    public Camera portalView;
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
}
