using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerButtonController : MonoBehaviour
{
    private Button myButton;
    private Image backgroundImage;
    private bool isBuild = false;
    private bool isPress = false;

    [Header("Color after successful placement")]
    public Color buildedColor = Color.red;

    [Header("Color when not yet placed")]
    public Color unbuildedColor = Color.green;

    [Header("Prefab to spawn")]
    public GameObject prefabToSpawn;

    [Header("Height offset when spawning prefab")]
    public float heightOffset = 1.0f;

    public GameObject mainCam;

    public GameObject canvasParent;

    void Awake()
    {
        // Get the Button and Image components on this GameObject
        myButton = GetComponent<Button>();
        backgroundImage = GetComponent<Image>();

        if (myButton != null)
        {
            myButton.onClick.RemoveAllListeners(); // Clear previous OnClick()
            myButton.onClick.AddListener(OnButtonClicked); // Assign our own OnClick
        }
    }

    public void OnButtonClicked()
    {
        if (!isBuild && !isPress)
        {
            isPress = true;
            TowerPlacementManager.Instance.SetSelectedButton(this);
            Debug.Log($"{gameObject.name} clicked successfully!");

            SpawnPrefabAtSelf();
        }
        else
        {
            Debug.Log($"{gameObject.name} is already built and cannot be clicked again!");
            return;
        }
    }

    public void SetisPressFalse()
    {
        isPress = false;
        Debug.Log($"isPress set to false!");
    }

    public void SetIsBuildTrue()
    {
        isBuild = true;
        backgroundImage.color = buildedColor;
        Debug.Log($"{gameObject.name} construction completed!");
    }

    public void SetIsBuildFalse()
    {
        isBuild = false;
        isPress = false;
        backgroundImage.color = unbuildedColor;
        Debug.Log($"{gameObject.name} reset to false state!");
    }

    private void SpawnPrefabAtSelf()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("prefabToSpawn is not set. Cannot spawn prefab!");
            return;
        }

        if (mainCam == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return;
        }

        Vector3 worldPos = transform.position + new Vector3(0, heightOffset, 0);
        Debug.Log("Local Position: " + transform.localPosition);
        Debug.Log("World Position: " + transform.position);

        // Spawn the prefab
        GameObject spawned = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
        spawned.name = transform.name + "_" + prefabToSpawn.name;

        // Calculate direction to face the camera (on horizontal plane)
        Vector3 dirToCamera = mainCam.transform.position - worldPos;
        dirToCamera.y = 0;
        if (dirToCamera != Vector3.zero) // Protect against zero direction
        {
            spawned.transform.rotation = Quaternion.LookRotation(dirToCamera);
        }

        if (canvasParent != null)
        {
            spawned.transform.SetParent(canvasParent.transform, worldPositionStays: true);
        }
        else
        {
            Debug.LogWarning("canvasParent is not assigned!");
        }

        // Attach the spawned object under this GameObject (e.g., Tower20)
        Debug.Log($"Spawned {spawned.name} at {worldPos} under {gameObject.name}");
    }

    public Vector3 getButtonPosition()
    {
        Debug.Log("Button Pos: " + transform.position + ", type: " + transform.position.GetType());
        return transform.position;
    }
}
