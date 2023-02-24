using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioID
    {
        AK47_Shoot = 0,
        AK47_Reload,
        NUM_AUDIO,
    }
    public AudioSource audioSource;
    public List<AudioClip> Clips;
    public float Volume = 1.0f;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetCurrentAudioSourceClip(AudioID audioID)
    {
        if ((int)audioID < Clips.Count)
            audioSource.clip = Clips[(int)audioID];
        else
            audioSource.clip = null;
    }
    public void PlayAudio(AudioID audioID, float Vol = 1.0f)
    {
        this.SetCurrentAudioSourceClip(audioID);
        if (audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip, Volume * Vol);
        }
    }
}
