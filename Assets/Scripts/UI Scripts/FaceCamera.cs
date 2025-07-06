using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
     private Camera targetCamera;

    void Start()
    {
        targetCamera = Camera.main;
        if (targetCamera == null)
        {
            Debug.LogError("找不到 Main Camera！請確認Main Camera有加Tag！");
        }
    }

    void Update()
    {
        if (targetCamera == null) return;

        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }
}
