using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetMovement : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.forward;
    public float speed = 5f;

    void Update()
    {
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }
}
