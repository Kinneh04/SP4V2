using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{
    public GameObject threeDAudioSourcePrefab;
    public enum AudioID
    {
        AK47_Shoot = 0,
        AK47_Reload,
        Pistol_Shoot,
        SMG_Shoot,
        Sniper_Shoot,
        Hitmarker,
        Pickup,
        Drop,
        Looting,
        Chewing,
        PistolPickup,
        ShotgunPickup,
        SMGPickup,
        GeneralPickup,
        Building,
        Click,
        Select,
        UpgradeStone,
        DestroyBuilding,
        DoorOpen,
        LockBtnPress,
        LockSuccess,
        LockFail,
        RepairBuilding,
        C4_Countdown,
        M1911_Shoot,
        M1911_Reload,
        BoltActionRifle_Shoot,
        BoltActionRifle_Reload,
        RocketLauncher_Shoot,
        RocketLauncher_Reload,
        Explosion,
        C4_Armed,
        MP5A4_Shoot,
        MP5A4_Reload,
        Bow_Charge,
        Bow_Shoot,
        Arrow_Land,
        HandmadeShotgun_Shoot,
        Remington870_Pump,
        Remington870_Shoot,
        Shotgun_Reload,
        Revolver_Shoot,
        Revolver_Reload,
        Picking,
        ItemWoosh,
        RockHit,
        WoodHit,
        Thunder,
        Ambience,
        Fox,
        Cow,
        Wolf,
        Tank,
        Scientist,
        Craft,
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

    [PunRPC]
    public void MultiplayerPlay3DAudio(int audioID, float Vol = 1.0f, Vector3 audioPos = default(Vector3))
    {
        // Create empty GO at position
        AudioSource threeDAudioSource = Instantiate(threeDAudioSourcePrefab, audioPos, Quaternion.identity).GetComponent<AudioSource>();
        threeDAudioSource.clip = Clips[audioID];
        threeDAudioSource.PlayOneShot(threeDAudioSource.clip, Vol);
    }
}
