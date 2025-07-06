using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] waypoints;

    public Transform[] GetPath()
    {
        return waypoints;
    }
}
