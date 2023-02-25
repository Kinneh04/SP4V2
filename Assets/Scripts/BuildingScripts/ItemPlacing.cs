using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPlacing : MonoBehaviour
{
    public Transform cam;
    public RaycastHit hit;
    public GameObject ObjectToPlace;
    bool canPlace = false;
    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("Camera").transform;
    }

    public void PlaceItem()
    {
        if (canPlace)
        {
            if (GetComponent<ItemInfo>().NetworkedReplacement)
            {
                PhotonNetwork.Instantiate(ObjectToPlace.name, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(ObjectToPlace, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
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
        if (Input.GetMouseButton(1))
        {
            print("Rotating!");
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z + 90);
        }
      
    }

  
}
