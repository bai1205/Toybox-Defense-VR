using UnityEngine;

public class WaypointGroundAligner : MonoBehaviour
{
    public LayerMask groundLayer;

    void Start()
    {
        RaycastHit hit;
        foreach (Transform child in transform)
        {
            Vector3 rayOrigin = child.position + Vector3.up * 100f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                child.position = hit.point;
            }
        }
    }
}
