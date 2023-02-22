using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropdownHolder : MonoBehaviour
{
    public static DropdownHolder instance;

    public TMP_Dropdown dropdown;
    public float renderDistance;
    GameObject[] grassObjects;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        grassObjects = GameObject.FindGameObjectsWithTag("Grass");
    }

    public void OnDropdownValueChanged()
    {
        switch (dropdown.value)
        {
            case 0: // High
                renderDistance = 200f;
                ShowGrass(true);
                break;
            case 1: // Medium
                renderDistance = 150f;
                ShowGrass(true);
                break;
            case 2: // Low
                renderDistance = 125f;
                ShowGrass(false);
                break;
        }

        // Find the player camera in the scene and set its render distance
       
    }
    private void ShowGrass(bool show)
    {
        foreach (GameObject grassObject in grassObjects)
        {
           if(show)
           {
                grassObject.SetActive(true);
           }
           else grassObject.SetActive(false);
        }
    }
    public void CallGraphicsChange()
    {
        GameObject playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        if (playerCamera != null)
        {
            Camera mainCamera = playerCamera.GetComponent<Camera>();
            if (mainCamera != null)
            {
                mainCamera.farClipPlane = renderDistance;
            }
        }
    }
}
