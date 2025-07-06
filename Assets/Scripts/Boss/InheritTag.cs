using UnityEngine;

public class InheritTag : MonoBehaviour
{
    private void Awake()
    {
        // Recursively set the tag for all child objects
        SetChildrenTag(transform, tag);
    }

    private void SetChildrenTag(Transform parent, string parentTag)
    {
        foreach (Transform child in parent)
        {
            child.tag = parentTag; // Set the current child's tag
            SetChildrenTag(child, parentTag); // Recursively process child objects
        }
    }
}
