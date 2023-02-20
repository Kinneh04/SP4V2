using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomMusic : MonoBehaviour
{
    public List<AudioSource> sources = new List<AudioSource>();
    public AudioSource currentlyPlayingSong;

    private void Start()
    {
        PlayRandomSong();
    }

    private void Update()
    {
        if (!currentlyPlayingSong.isPlaying)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        int i = Random.Range(0, sources.Count);
        currentlyPlayingSong = sources[i];
        currentlyPlayingSong.Play();
    }
}
