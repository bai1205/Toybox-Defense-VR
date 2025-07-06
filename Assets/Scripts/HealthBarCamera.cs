using UnityEngine;

public class HealthBarFaceCamera : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found. Please ensure the camera is tagged as 'MainCamera'!");
        }
    }

    void LateUpdate()
    {
        if (camTransform != null)
        {
            // Make the health bar face the camera
            transform.forward = camTransform.forward;
        }
    }
}
