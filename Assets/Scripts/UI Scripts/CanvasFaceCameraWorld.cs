using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCameraWorld : MonoBehaviour
{
    private Transform camTrans;
    private Transform localTrans;

    void Start()
    {
        localTrans = GetComponent<Transform>();
        if (Camera.main != null)
        {
            camTrans = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("CanvasFaceCameraWorld: Main Camera not found. Please ensure a camera is tagged as MainCamera.");
        }
    }

    void Update()
    {
        if (camTrans == null) return;

        // Keep the canvas at the same height, rotate only on the horizontal plane to face the camera
        Vector3 targetPos = camTrans.position;
        targetPos.y = localTrans.position.y;

        localTrans.LookAt(targetPos);
    }
}
