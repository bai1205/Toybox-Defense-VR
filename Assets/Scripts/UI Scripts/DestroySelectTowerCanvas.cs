using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelectTowerCanvas : MonoBehaviour
{
    public void DeleteAllSelectTowerCanvas()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Debug.Log(obj.name);
            if (obj.name == "SelectTowerCanvas(Clone)")
            {
                Destroy(obj);
            }
        }
    }
}
