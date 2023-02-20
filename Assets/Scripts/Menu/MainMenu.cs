using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject ServerRooms;
    public GameObject SettingsPage;

    public float bgmVolume = 1.0f, sfxVolume = 1.0f; // the default volume value
    public Slider BGMslider;
    public Slider SFXSlider;

    void Start()
    {
        // find all the AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // loop through each AudioSource to check if it's a BGM
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.CompareTag("BGM"))
            {
                // set the volume of the BGM to the default value
                audioSource.volume = bgmVolume;
            }
            else if (audioSource.CompareTag("SFX"))
            {
                audioSource.volume = sfxVolume;
            }
        }
    }

    public void SetBGMVolume()
    {
        // update the BGM volume based on the slider value
        float Bvalue = SFXSlider.value / SFXSlider.maxValue;
        sfxVolume = Bvalue;
        float value = BGMslider.value / BGMslider.maxValue;
        bgmVolume = value;
        // find all the AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // loop through each AudioSource to check if it's a BGM
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.CompareTag("BGM"))
            {
                // set the volume of the BGM to the updated value
                audioSource.volume = bgmVolume;
            }
            else if (audioSource.CompareTag("SFX"))
            {
                // set the volume of the BGM to the updated value
                audioSource.volume = sfxVolume;
            }
        }
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OpenServerRooms()
    {
        ServerRooms.SetActive(true);
        SettingsPage.SetActive(false);
    }

    public void OpenSettings()
    {
        ServerRooms.SetActive(false);
        SettingsPage.SetActive(true);
    }
}
