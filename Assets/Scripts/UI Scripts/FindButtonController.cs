using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindButtonController : MonoBehaviour
{
    public static void FindButtonAndSetBuildFalse(GameObject tower)
    {
        // First, get the tower's name
        string fullName = tower.name;

        // Find the position of the first "_"
        int underscoreIndex = fullName.IndexOf("_");

        if (underscoreIndex == -1)
        {
            Debug.LogError("Invalid name format: '_' not found. Unable to extract the original button name!");
            return;
        }

        // Extract the part before "_"
        string buttonName = fullName.Substring(0, underscoreIndex);
        Debug.Log($"Parsed button name: {buttonName}");

        // Try to find the GameObject with that name in the scene
        GameObject buttonObject = FindInScene(buttonName);
        if (buttonObject == null)
        {
            Debug.LogError($"Could not find object with name {buttonName}!");
            return;
        }

        // Try to get the TowerButtonController component
        TowerButtonController buttonController = buttonObject.GetComponent<TowerButtonController>();
        if (buttonController == null)
        {
            Debug.LogError($"TowerButtonController component not found on object {buttonName}!");
            return;
        }

        // If successful, call SetIsBuildFalse
        buttonController.SetIsBuildFalse();
    }

    public static void FindButtonAndSetPressFalse(GameObject tower)
    {
        // First, get the tower's name
        string fullName = tower.name;

        // Find the position of the first "_"
        int underscoreIndex = fullName.IndexOf("_");

        if (underscoreIndex == -1)
        {
            Debug.LogError("Invalid name format: '_' not found. Unable to extract the original button name!");
            return;
        }

        // Extract the part before "_"
        string buttonName = fullName.Substring(0, underscoreIndex);
        Debug.Log($"Parsed button name: {buttonName}");

        // Try to find the GameObject with that name in the scene
        GameObject buttonObject = FindInScene(buttonName);
        if (buttonObject == null)
        {
            Debug.LogError($"Could not find object with name {buttonName}!");
            return;
        }

        // Try to get the TowerButtonController component
        TowerButtonController buttonController = buttonObject.GetComponent<TowerButtonController>();
        if (buttonController == null)
        {
            Debug.LogError($"TowerButtonController component not found on object {buttonName}!");
            return;
        }

        // If successful, call SetisPressFalse
        buttonController.SetisPressFalse();
    }

    private static GameObject FindInScene(string name)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }
}
