using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance;
    public TowerButtonController selectedButton; // 記錄目前點擊的格子按鈕

    private void Awake()
    {
        Instance = this;
    }

    public void SetSelectedButton(TowerButtonController button)
    {
        selectedButton = button;
    }

    public TowerButtonController GetSelectedButton()
    {
        return selectedButton;
    }
}
