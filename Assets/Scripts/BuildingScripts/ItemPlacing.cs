using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPlacing : MonoBehaviour
{
    public AudioManager audioManager;
    public Transform cam;
    public RaycastHit hit;
    private Vector3 currentRot = Vector3.zero;
    public GameObject ObjectToPlace;
    bool canPlace = false;
    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("Camera").transform;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        gameObject.transform.rotation = Quaternion.identity;
    }

    public virtual bool PlaceItem()
    {
        if (canPlace)
        {
            if (GetComponent<ItemInfo>().NetworkedReplacement)
            {
                PhotonNetwork.Instantiate(ObjectToPlace.name, transform.position, Quaternion.identity);
                audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.AllViaServer, AudioManager.AudioID.Building, 1.0f, transform.position);
                PhotonNetwork.Instantiate("PlacingSmoke", transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(ObjectToPlace, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview")))
        {
            if (hit.transform != transform)
            {
                canPlace = true;
                gameObject.transform.position = hit.point;

                Renderer renderer = GetComponentInChildren<Renderer>();
                Color redTranslucent = new Color(0.58f, 0.9f, 1f, 0.5f);
                renderer.material.color = redTranslucent;
            }
        }
        else
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            Color redTranslucent = new Color(1f, 0f, 0f, 0.5f);
            renderer.material.color = redTranslucent;
            canPlace = false;
        }
/*        if (Input.GetMouseButton(1))
        {
            currentRot += new Vector3(0, 0, 90);
            gameObject.transform.localEulerAngles = currentRot;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRot += new Vector3(0, 90, 0);
            gameObject.transform.localEulerAngles = currentRot;
        }*/
      
    }

  
}
