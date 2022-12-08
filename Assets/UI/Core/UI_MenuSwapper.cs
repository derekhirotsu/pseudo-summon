using PseudoSummon.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuSwapper : MonoBehaviour
{
    [SerializeField] private GameObject[] menuPanels;

    public void ToggleMenu(int menuIndex = 0)
    {
        foreach (GameObject menu in menuPanels)
        {
            menu.SetActive(false);
        }

        menuPanels[menuIndex].SetActive(true);
    }

    public void ToggleMenus(params int[] menuIndices)
    {
        foreach (GameObject menu in menuPanels)
        {
            menu.SetActive(false);
        }

        foreach (int index in menuIndices)
        {
            menuPanels[index].SetActive(true);
        }
    }
}
