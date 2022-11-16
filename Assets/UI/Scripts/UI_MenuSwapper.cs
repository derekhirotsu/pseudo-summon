using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuSwapper : MonoBehaviour
{

    [SerializeField] protected GameObject[] menuPanels;

    void Awake() {
        ToggleMenu(1);
    }


    public void ToggleMenu(int menuId = 0) {

        // Case index out of bounds
        if (menuId > menuPanels.Length) {
            Debug.LogWarning("A menu of index " + (menuId - 1) + " does not exist. ");
            return;
        }

        // Disable menu panels
        foreach (GameObject menu in menuPanels) {
            menu.SetActive(false);
        }

        // Case index is 0
        if (menuId == 0) {
            return;
        }

        DisplayMenu(menuId-1);
    }

    protected void DisplayMenu(int index) {
        // Enable selected menu given it's not null
        GameObject selectedMenu = menuPanels[index];
        if (selectedMenu != null) {
            selectedMenu.SetActive(true);
        }
    }
}
