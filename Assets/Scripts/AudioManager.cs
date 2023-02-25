using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{
    public enum AudioID
    {
        AK47_Shoot = 0,
        AK47_Reload,
        SMG_Shoot,
        Pistol_Shoot,
        Sniper_Shoot,
        Hitmarker,
        NUM_AUDIO,
    }
    public AudioSource audioSource;
    public GameObject GraphicsLoader;
    public List<AudioClip> Clips;
    public float Volume = 1.0f;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        GraphicsLoader = GameObject.Find("GraphicsLoader");
        Volume = GraphicsLoader.GetComponent<DropdownHolder>().sfxVolume;
    }
    public void SetCurrentAudioSourceClip(int audioID)
    {
        if (audioID < Clips.Count)
            audioSource.clip = Clips[audioID];
        else
            audioSource.clip = null;
    }
    public void PlayAudio(int audioID, float Vol = 1.0f)
    {
        this.SetCurrentAudioSourceClip(audioID);
        if (audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip, Volume * Vol);
        }
    }
    [PunRPC]
    public void MultiplayerPlayAudio(int audioID, float Vol = 1.0f)
    {
        PlayAudio(audioID, Vol);
    }
}
