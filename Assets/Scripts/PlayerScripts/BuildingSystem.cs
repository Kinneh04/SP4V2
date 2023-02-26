using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuildingSystem : MonoBehaviour
{
    public List<BuildObjects> objects = new List<BuildObjects>();
    public BuildObjects currentObject;
    private Vector3 currentPos;
    private Vector3 currentRot = Vector3.zero;
    private bool isTranslated = false;
    public Transform currentPreview;

    public AudioManager audioManager;
    public PlayerProperties pp;
    public InventoryManager im;
    public GameObject woodObj;
    public CreatePopup cp;
    public GameObject smokeVFX;

    // Variables for raycast
    public Transform cam;
    public RaycastHit hit;
    public LayerMask layer; // To assign it to exclude BuildPreview in raycast checks
    public LayerMask buildableLayer;

    public float offset = 1.0f;
    public float gridSize = 1.0f;

    public bool IsBuilding = false;
    private bool IsChoosingObj = false;
    public float BuildCooldown;
    public GameObject menuObject;

    void Start()
    {
        BuildCooldown = 0.0f;
        // Start off with a foundation
        currentObject = objects[0];
        ChangeCurrentBuilding(0);
        // Disable objects until using Building Plan
        menuObject.SetActive(false);
        currentPreview.gameObject.SetActive(false);

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (BuildCooldown > 0.0f)
            BuildCooldown -= Time.deltaTime;

        if (!IsBuilding || !GetComponent<PhotonView>().IsMine)
            return;

        if (!IsChoosingObj)
            StartPreview();

        // Open choosing menu
        if (Input.GetMouseButtonDown(1) && !menuObject.activeSelf)
        {
            SetMenuActive(true);
        }
        else if (!Input.GetMouseButton(1) && menuObject.activeSelf)
        {
            audioManager.PlayAudio((int)AudioManager.AudioID.Select);
            ChangeCurrentBuilding(menuObject.GetComponent<CircularMenu>().CurrMenuItem);
            SetMenuActive(false);
        }
    }

    public void SetMenuActive(bool isOpen)
    {
        if (isOpen)
        {
            menuObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            IsChoosingObj = true;
        }
        else
        {
            // Hide choosing menu the moment right click is no longer held, and select the currently selected object
            menuObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            IsChoosingObj = false;
        }
    }

    public void ChangeCurrentBuilding(int curr)
    {
        currentObject = objects[curr];
        if (currentPreview != null)
            Destroy(currentPreview.gameObject);
        GameObject curPrev = Instantiate(currentObject.preview, currentPos, Quaternion.Euler(currentRot));
        currentPreview = curPrev.transform;
    }

    public void StartPreview()
    {
        StartPreview(currentPreview);
    }

    // Overrides for HammerSystem
    public void StartPreview(Transform currPreview)
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview"), layer))
        {
            if (hit.transform != transform)
                ShowPreview(hit, currPreview);
        }
    }

    public void ShowPreview(RaycastHit hit2, Transform currPreview)
    {
        currentPos = hit2.point;
        currentPos -= Vector3.one * offset;
        currentPos /= gridSize;
        currentPos = new Vector3(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y), Mathf.Round(currentPos.z));
        currentPos *= gridSize;
        currentPos += Vector3.one * offset;
        currentPos.y += currPreview.localScale.y * 0.5f;
        currPreview.position = currentPos; // snap preview to current grid

        if (currentObject.name == "Door")
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview"), buildableLayer))
            {
                // Snap doors to doorframes when player is looking at them
                if (hit.collider.gameObject.CompareTag("NormalStructure"))
                {
                    StructureObject structure = hit.collider.GetComponent<StructureObject>();
                    if (structure && structure.type == StructureTypes.doorway)
                    {
                        currPreview.position = new Vector3(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y, hit.collider.gameObject.transform.position.z);
                        currPreview.Translate(new Vector3(-0.64f, 0.569f, 1.5f));
                        currPreview.rotation = hit.collider.gameObject.transform.rotation;
                        currentPos = currPreview.position;
                        currentRot = currPreview.localEulerAngles;
                        currentPreview.GetComponent<PreviewObject>().IsBuildable = true;
                        return;
                    }
                }
            }
            currentPreview.GetComponent<PreviewObject>().IsBuildable = false;
            isTranslated = true;
        }
        else if (currentObject.name == "Floor")
        {
            // Snap to top of walls when looking at them, and next to other floors
        }

        isTranslated = false;

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRot += new Vector3(0, 90, 0);
            currPreview.localEulerAngles = currentRot;
        }
    }

    public void SetIsBuilding(bool building)
    {
        if (building)
        {
            GameObject curPrev = Instantiate(currentObject.preview, currentPos, Quaternion.Euler(currentRot));
            currentPreview = curPrev.transform;
        }
        else
        {
            if (currentPreview != null)
                Destroy(currentPreview.gameObject);
        }

        IsBuilding = building;
    }

    public void Build()
    {
        if (BuildCooldown > 0.0f)
            return;

        if (IsChoosingObj)
            return;

        if (pp.isBuildingDisabled)
        {
            cp.CreateResourcePopup("Building disabled!", 0);
            return;
        }

        PreviewObject po = currentPreview.GetComponent<PreviewObject>();
        if (po.IsBuildable)
        {
            if (im.GetAmmoQuantity(ItemInfo.ItemID.Wood) < currentObject.wood)
            {
                cp.CreateResourcePopup("Not enough wood!", 0);
            }
            else
            {
                cp.CreateResourcePopup("Wood", currentObject.wood);
                im.RemoveQuantity(woodObj.GetComponent<ItemInfo>(), currentObject.wood);
                GameObject newObj = PhotonNetwork.Instantiate(currentObject.name, currentPos, Quaternion.Euler(currentRot));
                if (isTranslated)
                {
                    newObj.transform.localPosition = currentPreview.transform.localPosition;
                    newObj.transform.localEulerAngles = currentPreview.transform.localEulerAngles;
                }
                Instantiate(smokeVFX, newObj.transform);
                audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlayAudio", RpcTarget.AllViaServer, AudioManager.AudioID.Building, 1f);

                switch (currentObject.name)
                {
                    case "Walls":
                    case "Window":
                    case "DoorFrame":
                    case "Floor":
                        {
                            foreach (Collider col in currentPreview.gameObject.GetComponent<PreviewObject>().nextToCol)
                            {
                                if (col.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
                                {
                                    col.gameObject.GetComponent<StructureObject>().dependentStructures.Add(newObj);
                                }
                                else if (col.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                                {
                                    col.gameObject.GetComponentInParent<StructureObject>().dependentStructures.Add(newObj);
                                }
                            }
                        }
                        break;
                    case "Door":
                        {
                            foreach (Collider col in currentPreview.gameObject.GetComponent<PreviewObject>().nextToCol)
                            {
                                Debug.Log("COL: " + col.name);
                                StructureObject so = null;
                                if (col.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
                                {
                                    so = col.gameObject.GetComponent<StructureObject>();
                                }
                                else if (col.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                                {
                                    so = col.gameObject.GetComponentInParent<StructureObject>();
                                }

                                if (!so || so.type != StructureTypes.doorway)
                                    return;

                                Debug.Log("ADDED: " + newObj.name);
                                so.dependentStructures.Add(newObj);
                            }
                        }
                        break;
                }

                if (currentObject.name == "Door")
                {
                    newObj.GetComponent<DoorStructure>().PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
                }

                newObj.GetComponent<StructureObject>().PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
            }
        }
        BuildCooldown = 1.0f;
    }
}

[System.Serializable]
public class BuildObjects
{
    public string name;
    public int wood;
    public GameObject preview;
    public GameObject prefab;
    public GameObject selectedPrefab;
}
