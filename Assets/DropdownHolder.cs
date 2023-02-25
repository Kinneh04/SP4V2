using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropdownHolder : MonoBehaviour
{
    public static DropdownHolder instance;
    public bool DontDestroyOnLoad;
    public TMP_Dropdown dropdown;
    public Slider sfxSlider;
    public Slider musicSlider;
    public float renderDistance = 600;
    public float musicVolume;
    public float sfxVolume;
    GameObject[] grassObjects;

    public Camera cam;
    public AudioManager AM;
    public Terrain terrain;
    private void Awake()
    {
        if (DontDestroyOnLoad)
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        grassObjects = GameObject.FindGameObjectsWithTag("Grass");
        renderDistance = 600f;
        sfxSlider.value = 0.5f;
        musicSlider.value = 0.5f;
        musicVolume = musicSlider.value;
        sfxVolume = sfxSlider.value;

    }

    public void LiveUpdateGraphicalSettings()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        cam.farClipPlane = renderDistance;
        if(dropdown.value == 2)
        {
            TerrainData terrainData = terrain.terrainData;
            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            terrainData.SetDetailLayer(0, 0, 0, detailMap);
        }
        AM = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        AM.Volume = sfxSlider.value;
    }

    public void RecordSliderValues()
    {
        musicVolume = musicSlider.value;
        sfxVolume = sfxSlider.value;
    }

    public void OnDropdownValueChanged()
    {
        switch (dropdown.value)
        {
            case 0: // High
                renderDistance = 600f;
                ShowGrass(true);
                break;
            case 1: // Medium
                renderDistance = 500f;
                ShowGrass(true);
                break;
            case 2: // Low
                renderDistance = 400f;
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
