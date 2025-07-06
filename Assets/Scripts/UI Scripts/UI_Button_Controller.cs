using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Button_Controller : MonoBehaviour
{
    public GameObject settingCanvas;
    public GameObject mainCanvas;
    public GameObject towerCanvas;
    public GameObject buildingCanvas;
    public GameObject upgradeCanvas;
    public GameObject selectCanvas;
    public static UI_Button_Controller Instance;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake(){
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SwitchCanvas(GameObject activeCanvas)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (settingCanvas != null) settingCanvas.SetActive(false);
        if (towerCanvas != null) towerCanvas.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.SetActive(false);
        if (buildingCanvas != null) buildingCanvas.SetActive(false);
        if (selectCanvas != null ) selectCanvas.SetActive(false);
        Debug.Log("SwitchCanvas");
        if (activeCanvas != null) activeCanvas.SetActive(true);
    }
    public void SwitchCanvasToNor()
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (settingCanvas != null) settingCanvas.SetActive(false);
        if (towerCanvas != null) towerCanvas.SetActive(false);
        if (buildingCanvas != null) buildingCanvas.SetActive(false);
        if (selectCanvas != null ) selectCanvas.SetActive(false);

        if (mainCanvas != null) mainCanvas.SetActive(true);
    }
    public void OnBackArrowButtonClick()
    {
        SwitchCanvas(mainCanvas);
    }
    public void OnSettingButtonClick()
    {
        Debug.Log("Setting Button clicked");
        SwitchCanvas(settingCanvas);
    }
    public void OnTowerButtonClick()
    {
        
        SwitchCanvas(towerCanvas);
        if(buildingCanvas != null) buildingCanvas.SetActive(true);
        if (selectCanvas != null ) selectCanvas.SetActive(true);
    }
    public void OnUpgradeButtonClick()
    {
        SwitchCanvas(upgradeCanvas);
    }

    public void OnCancelBuildingButtonClick()
    {
        foreach (Transform child in selectCanvas.transform)
        {   
            FindButtonController.FindButtonAndSetPressFalse(child.gameObject);
            Destroy(child.gameObject);
        }
        SwitchCanvas(mainCanvas);

    }

    public void OnBacktoMainClick(){
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRestartClick(){
        Scene currentScene = SceneManager.GetActiveScene();
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentScene.name);
    }





}