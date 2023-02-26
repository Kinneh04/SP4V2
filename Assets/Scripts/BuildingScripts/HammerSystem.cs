using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class HammerSystem : MonoBehaviour
{
    private GameObject prevObject;
    private GameObject currentObject;
    private GameObject currentPreview;
    private StructureObject currStructure;
    private SelectedObject selectedObject;
    private int currentAction;

    // Variables for raycast
    public BuildingSystem bs;
    public Transform cam;
    public LayerMask layer; // To assign it to exclude BuildPreview in raycast checks
    private RaycastHit hit;

    // To prevent raycast from being reset constantly
    bool hasHit = false;
    float prevMouseCoordsX;
    float prevMouseCoordsY;

    public PlayerProperties pp;
    public InventoryManager im;
    public GameObject woodObj;
    public GameObject stoneObj;
    public CreatePopup cp;
    public AudioManager audioManager;

    private bool IsUsingHammer = false;
    private bool IsPickingUp = false;
    public float ActionCooldown = 1.0f;
    public GameObject menuObject;
    public Material stoneMaterial;

    public List<Sprite> actionIcons = new List<Sprite>();
    private PhotonView pv;

    void Start()
    {
        // Start off with a foundation
        currentAction = 0;
        ChangeCurrentAction(0);
        prevObject = null;
        currentObject = null;
        selectedObject = null;
        currStructure = null;
        currentPreview = null;
        // Disable objects until using Building Plan
        menuObject.SetActive(false);
        pv = GetComponent<PhotonView>();

        prevMouseCoordsX = Input.GetAxis("Mouse X");
        prevMouseCoordsY = Input.GetAxis("Mouse Y");
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (ActionCooldown > 0.0f)
            ActionCooldown -= Time.deltaTime;

        if (!IsUsingHammer || !pv.IsMine)
            return;

        if (selectedObject != null && currStructure != null && currStructure.isDamaged)
        {
            selectedObject.slider.value = currStructure.stability;
            selectedObject.stabilityLabel.text = Math.Round(currStructure.stability, 2) + "%";
        }

        if (IsPickingUp)
        {
            bs.StartPreview(currentPreview.transform);
        }
        else
        {
            StartSelect();

            // Open choosing menu
            if (Input.GetMouseButtonDown(1) && !menuObject.activeSelf)
            {
                SetMenuActive(true);
            }
            else if (!Input.GetMouseButton(1) && menuObject.activeSelf)
            {
                ChangeCurrentAction(menuObject.GetComponent<CircularMenu>().CurrMenuItem);
                SetMenuActive(false);
            }
        }
    }

    public void SetMenuActive(bool isOpen)
    {
        if (isOpen)
        {
            menuObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Hide choosing menu the moment right click is no longer held, and select the currently selected action
            menuObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ChangeCurrentAction(int curr)
    {
        currentAction = curr;
        if (selectedObject != null)
        {
            UpdateSelectedUI();
        }
    }

    private void UpdateSelectedUI()
    {
        selectedObject.actionImage.sprite = actionIcons[currentAction];
        switch (currentAction)
        {
            case 0: // Pickup, write whether object is pickable
                {
                    if (currStructure.pickupCooldown > 0.0f)
                    {
                        selectedObject.actionImage.color = Color.white;
                        selectedObject.costLabel.text = Math.Round(currStructure.pickupCooldown, 1) + "s left to pickup";
                    }
                    else
                    {
                        selectedObject.actionImage.color = Color.black;
                        selectedObject.costLabel.text = "Pickup time expired";
                    }
                    break;
                }
            case 1: // Repair, list the cost to repair
                {
                    if (currStructure.stability == 100)
                    {
                        selectedObject.actionImage.color = Color.black;
                        selectedObject.costLabel.text = "Already at max stability";
                    }
                    else
                    {
                        selectedObject.actionImage.color = Color.white;
                        string type = "x Wood";
                        if (currStructure.isUpgraded)
                            type = "x Stone";

                        switch (currStructure.type)
                        {
                            case StructureTypes.foundation:
                            case StructureTypes.wall:
                                selectedObject.costLabel.text = 10 + type;
                                break;
                            case StructureTypes.floor:
                            case StructureTypes.stairs:
                                selectedObject.costLabel.text = 5 + type;
                                break;
                            case StructureTypes.doorway:
                            case StructureTypes.window:
                                selectedObject.costLabel.text = 7 + type;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                }
            case 2: // Upgrade, list the cost to upgrade
                {
                    if (currStructure.isUpgraded)
                    {
                        selectedObject.actionImage.color = Color.black;
                        selectedObject.costLabel.text = "Already upgraded";
                    }
                    else
                    {
                        selectedObject.actionImage.color = Color.white;
                        switch (currStructure.type)
                        {
                            case StructureTypes.foundation:
                            case StructureTypes.wall:
                                selectedObject.costLabel.text = 40 + "x Stone";
                                break;
                            case StructureTypes.floor:
                            case StructureTypes.stairs:
                                selectedObject.costLabel.text = 15 + "x Stone";
                                break;
                            case StructureTypes.doorway:
                            case StructureTypes.window:
                                selectedObject.costLabel.text = 25 + "x Stone";
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                }
            default:
                selectedObject.costLabel.text = "";
                selectedObject.actionImage.color = Color.white;
                break;
        }
    }

    public void StartSelect()
    {
/*        CheckHasMouseMoved();

        if (hasHit)
            return;*/

        if (Physics.Raycast(cam.position, cam.forward, out hit, float.PositiveInfinity))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
            {
                if (hit.transform != transform)
                    ShowSelected(hit);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Buildable"))
            {
                // Do a second raycast this time limiting to only BuildableParent
                if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview"), layer))
                {
                    if (hit.transform != transform)
                        ShowSelected(hit);
                }
            }
            else if (hit.collider.gameObject.layer != LayerMask.NameToLayer("BuildPreview"))
            {
                ResetPointers();
            }
        }
        else // If not looking at anything at all
        {
            ResetPointers();
        }
    }

    public void ShowSelected(RaycastHit hit2)
    {
        // Reset previous object and set them to active
        prevObject = currentObject;
        currentObject = hit2.collider.gameObject;

        if (currentObject != prevObject 
            && currentObject.GetComponent<StructureObject>().type != StructureTypes.door
            && (currentObject.GetComponent<StructureObject>().PlayerID == PhotonNetwork.LocalPlayer.ActorNumber || pp.hasBuildingPrivilege))
        {
            hasHit = true;

            if (prevObject != null)
                prevObject.SetActive(true);
            if (selectedObject != null)
                Destroy(selectedObject.gameObject);

            Vector3 curRot = currentObject.transform.rotation.eulerAngles;
            Vector3 curPos = currentObject.transform.position;
            currStructure = currentObject.GetComponent<StructureObject>();
            GameObject newObj = Instantiate(currStructure.selectedPrefab, curPos, Quaternion.Euler(curRot));
            selectedObject = newObj.GetComponent<SelectedObject>();
            UpdateSelectedUI();
            // Update material if structure has been upgraded
            if (currStructure.isUpgraded)
            {
                foreach (Transform child in selectedObject.gameObject.transform) // Change look to stone
                {
                    if (child.GetComponent<Renderer>() == null)
                        return;

                    Material[] mats = child.GetComponent<Renderer>().materials;
                    mats[0] = stoneMaterial;
                    child.GetComponent<Renderer>().materials = mats;
                }
            }

            currentObject.SetActive(false);
        }
    }

    public void SetIsUsingHammer(bool isUsing)
    {
        // Also delete selection if no longer using
        if (!isUsing)
        {
            ResetPointers();
        }

        IsUsingHammer = isUsing;
    }

    private void ResetPointers()
    {
        if (currentObject != null)
        {
            currentObject.SetActive(true);
            currentObject = null;
        }
        if (selectedObject != null)
        {
            Destroy(selectedObject.gameObject);
            selectedObject = null;
        }
        prevObject = null;
        currStructure = null;
    }

    public void PerformAction()
    {
        if (ActionCooldown > 0.0f || currStructure == null || selectedObject == null)
            return;

        if (IsPickingUp)
        {
            PlaceStructure();
        }
        else
        {
            switch (currentAction)
            {
                case 0: // Pickup
                    {
                        if (currStructure.pickupCooldown > 0.0f)
                        {
                            if (currStructure.dependentStructures.Count > 0)
                            {
                                bool isDependant = false;
                                foreach (GameObject structure in currStructure.dependentStructures)
                                {
                                    if (structure != null) // Check whether every structure in list has been destroyed
                                    {
                                        isDependant = true;
                                        break;
                                    }
                                }

                                if (isDependant) // If there is at least one dependant undestroyed structure, show error and do not pickup
                                {
                                    cp.CreateResourcePopup("Dependant Structure", 0);
                                    return;
                                }                     
                            }

                            currentPreview = Instantiate(bs.objects[(int)currStructure.type].preview, selectedObject.gameObject.transform);
                            IsPickingUp = true;
                        }
                        break;
                    }
                case 1: // Repair
                    {
                        if (currStructure.stability != 100)
                        {
                            if (selectedObject.costLabel.text.EndsWith("x Wood"))
                            {
                                int cost = int.Parse(selectedObject.costLabel.text.Substring(0, selectedObject.costLabel.text.LastIndexOf("x Wood")));
                                if (im.GetAmmoQuantity(ItemInfo.ItemID.Wood) < cost)
                                {
                                    cp.CreateResourcePopup("Not enough wood!", 0);
                                }
                                else
                                {
                                    cp.CreateResourcePopup("Wood", cost);
                                    im.RemoveQuantity(woodObj.GetComponent<ItemInfo>(), cost);
                                    currStructure.gameObject.GetComponent<PhotonView>().RPC("RepairStructure", RpcTarget.AllViaServer, 25.0f);
                                    audioManager.PlayAudio((int)AudioManager.AudioID.RepairBuilding);
                                    PhotonNetwork.Instantiate("PlacingSmoke", currStructure.transform.position, currStructure.transform.rotation);

                                }
                            }
                            else if (selectedObject.costLabel.text.EndsWith("x Stone"))
                            {
                                int cost = int.Parse(selectedObject.costLabel.text.Substring(0, selectedObject.costLabel.text.LastIndexOf("x Stone")));
                                if (im.GetAmmoQuantity(ItemInfo.ItemID.Stone) < cost)
                                {
                                    cp.CreateResourcePopup("Not enough stone!", 0);
                                }
                                else
                                {
                                    cp.CreateResourcePopup("Stone", cost);
                                    im.RemoveQuantity(stoneObj.GetComponent<ItemInfo>(), cost);
                                    currStructure.gameObject.GetComponent<PhotonView>().RPC("RepairStructure", RpcTarget.AllViaServer, 25.0f);
                                    audioManager.PlayAudio((int)AudioManager.AudioID.RepairBuilding);
                                    PhotonNetwork.Instantiate("PlacingSmoke", currStructure.transform.position, currStructure.transform.rotation);
                                }
                            }
                        }
                        break;
                    }
                case 2: // Upgrade
                    {
                        if (!currStructure.isUpgraded)
                        {
                            if (selectedObject.costLabel.text.EndsWith("x Stone"))
                            {
                                int cost = int.Parse(selectedObject.costLabel.text.Substring(0, selectedObject.costLabel.text.LastIndexOf("x Stone")));
                                if (im.GetAmmoQuantity(ItemInfo.ItemID.Stone) < cost)
                                {
                                    cp.CreateResourcePopup("Not enough stone!", 0);
                                }
                                else
                                {
                                    cp.CreateResourcePopup("Stone", cost);
                                    im.RemoveQuantity(stoneObj.GetComponent<ItemInfo>(), cost);
                                    currStructure.gameObject.GetComponent<PhotonView>().RPC("UpgradeStructure", RpcTarget.AllViaServer);
                                    PhotonNetwork.Instantiate("PlacingSmoke", currStructure.transform.position, currStructure.transform.rotation);
                                    audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.AllViaServer, AudioManager.AudioID.UpgradeStone, 1.0f, currStructure.transform.position);
                                    // Also update current selected
                                    foreach (Transform child in selectedObject.gameObject.transform) // Change look to stone
                                    {
                                        if (child.GetComponent<MeshRenderer>() == null)
                                            return;

                                        Material[] mats = child.GetComponent<MeshRenderer>().materials;
                                        Material[] newMaterials = new Material[] { stoneMaterial, mats[1] };
                                        child.GetComponent<MeshRenderer>().materials = newMaterials;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 3: // Destroy
                    {
                        audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.AllViaServer, AudioManager.AudioID.DestroyBuilding, 1.0f, currentObject.transform.position);
                        pv.RPC("DestroyStructure", PhotonNetwork.CurrentRoom.GetPlayer(currentObject.GetComponent<StructureObject>().PlayerID), currentObject.GetComponent<PhotonView>().ViewID);
                        Destroy(selectedObject.gameObject);
                        selectedObject = null;
                        prevObject = null;
                        currStructure = null;
                        currentObject = null;
                    }
                    break;
                default:
                    break;
            }
        }

        ActionCooldown = 1.0f;
    }

    public void PlaceStructure()
    {
        if (currentPreview.GetComponent<PreviewObject>().IsBuildable)
        {
            currentObject.transform.SetPositionAndRotation(currentPreview.transform.position, currentPreview.transform.rotation);
            currentObject.SetActive(true);
            IsPickingUp = false;
            Destroy(currentPreview.gameObject);
            PhotonNetwork.Instantiate("PlacingSmoke", currentObject.transform.position, currentObject.transform.rotation);
            audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.AllViaServer, AudioManager.AudioID.Building, 1.0f, currentObject.transform.position);
        }
    }

    [PunRPC]
    public void DestroyStructure(int viewID)
    {
        // Also destroy structures dependent on this object
        foreach (GameObject structure in PhotonView.Find(viewID).gameObject.GetComponent<StructureObject>().dependentStructures)
        {
            if (structure != null) // null check for destroyed ones
            {
                PhotonView.Find(viewID).RPC("DestroyStructureObject", PhotonNetwork.CurrentRoom.GetPlayer(structure.GetComponent<StructureObject>().PlayerID), structure.GetComponent<PhotonView>().ViewID);
            }
        }

        if (PhotonView.Find(viewID).gameObject.GetComponent<StructureObject>().isUpgraded)
        {
            PhotonNetwork.Instantiate("DestroyStructureStone", PhotonView.Find(viewID).gameObject.transform.position, PhotonView.Find(viewID).gameObject.transform.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate("DestroyStructure", PhotonView.Find(viewID).gameObject.transform.position, PhotonView.Find(viewID).gameObject.transform.rotation);
        }
        PhotonNetwork.Destroy(PhotonView.Find(viewID));
    }

    private void CheckHasMouseMoved()
    {
        if (Input.GetAxis("Mouse X") != prevMouseCoordsX || Input.GetAxis("Mouse Y") != prevMouseCoordsY)
        {
            Debug.Log("MOUSE MOVED!");
            hasHit = false;
            prevMouseCoordsX = Input.GetAxis("Mouse X");
            prevMouseCoordsY = Input.GetAxis("Mouse Y");
        }
    }
}