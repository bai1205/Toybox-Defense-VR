using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTowerController : MonoBehaviour
{
    private Button myButton;
    private int towerBudget = 150;

    public GameObject prefabToSpawn;
    public GameObject getPosiObject;

    private TowerButtonController towerButtonController; 

    void Awake()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.RemoveAllListeners(); // 清除舊的 OnClick()
            myButton.onClick.AddListener(OnButtonClicked); // 綁自己的OnClick
        }

    }

    public void SetTowerButtonController(TowerButtonController controller)
    {
        towerButtonController = controller;
    }

    public void OnButtonClicked()
    {
        int currentMoney = CurrencyManager.Instance.GetCurrentMoney();
        bool canBuy = enoughMoney(currentMoney);
        if(canBuy) 
        {   
            CurrencyManager.Instance.SpendMoney(towerBudget);
            if (getPosiObject == null)
            {
                Debug.LogError("GetPosi GameObject not assigned!");
                return;
            }

            Vector3 spawnPos = getPosiObject.transform.position - new Vector3(-3.5f, 6.19889f, -1.5f);
            GameObject towerSpawn = Instantiate(prefabToSpawn,spawnPos,Quaternion.identity);
            towerSpawn.transform.localScale *= 2.2617282f;
            towerSpawn.transform.SetParent(null);
            Debug.Log("build");
            UI_Button_Controller.Instance.SwitchCanvasToNor();
            TowerButtonController selectedButton = TowerPlacementManager.Instance.GetSelectedButton();
            towerSpawn.name = selectedButton.gameObject.name + "_" +　prefabToSpawn.name ;
            if (selectedButton != null)
            {
                selectedButton.SetIsBuildTrue();
                TowerPlacementManager.Instance.SetSelectedButton(null); // 清空
            }

        }
        else
        {
            Debug.Log("You Do not have enough MOney");
            return;
        }
        transform.parent.gameObject.SetActive(false);
    }

    private bool enoughMoney(int currentMoney)
    {   
        return towerBudget <= currentMoney;
    }

}
