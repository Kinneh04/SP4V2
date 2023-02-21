using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class HammerSystem : MonoBehaviour
{
    private GameObject prevObject;
    private GameObject currentObject;
    private StructureObject currStructure;
    private SelectedObject selectedObject;
    private int currentAction;

    // Variables for raycast
    public BuildingSystem bs;
    public Transform cam;
    public LayerMask layer; // To assign it to exclude BuildPreview in raycast checks
    private RaycastHit hit;

    public InventoryManager im;
    public GameObject woodObj;
    public GameObject stoneObj;
    public CreatePopup cp;

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
        // Disable objects until using Building Plan
        menuObject.SetActive(false);
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (ActionCooldown > 0.0f)
            ActionCooldown -= Time.deltaTime;

        if (!IsUsingHammer || !pv.IsMine)
            return;

        if (IsPickingUp)
        {
            bs.StartPreview(selectedObject.gameObject.transform);
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
        selectedObject.slider.value = currStructure.stability;
        selectedObject.stabilityLabel.text = currStructure.stability + "%";
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
        if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview"), layer))
        {
            if (hit.transform != transform)
                ShowSelected(hit);
        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, LayerMask.NameToLayer("BuildPreview"))) // If looking at something that's not buildable
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BuildPreview")) // Do not count selectable
                return;

            ResetPointers();
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

        if (currentObject != prevObject && currentObject.GetComponent<StructureObject>().pv.IsMine)
        {
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
        if (ActionCooldown > 0.0f)
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
                            IsPickingUp = true;
                            // TODO Replace selectedObject with previewObjects because they do not have error checking!
                            selectedObject.gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0;
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
                                    currStructure.stability += 25;
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
                                    currStructure.stability += 25;
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
                                    currStructure.stability = 100;
                                    currStructure.isUpgraded = true;
                                    currStructure.gameObject.GetComponent<PhotonView>().RPC("UpgradeStructure", RpcTarget.AllViaServer);

                                    // Also update current selected
                                    foreach (Transform child in selectedObject.gameObject.transform) // Change look to stone
                                    {
                                        if (child.GetComponent<MeshRenderer>() == null)
                                            return;

                                        Material[] mats = child.GetComponent<MeshRenderer>().materials;
                                        Material[] newMaterials = new Material[] { stoneMaterial, mats[1] };
                                        Debug.Log(newMaterials.ToString());
                                        child.GetComponent<MeshRenderer>().materials = newMaterials;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 3: // Destroy
                    {
                        pv.RPC("DestroyStructure", RpcTarget.AllViaServer);
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
        currentObject.transform.SetPositionAndRotation(selectedObject.transform.position, selectedObject.transform.rotation);
        currentObject.SetActive(true);
        IsPickingUp = false;
        selectedObject.gameObject.GetComponentInChildren<CanvasGroup>().alpha = 1;
        Destroy(selectedObject.gameObject);
    }

    [PunRPC]
    public void DestroyStructure()
    {
        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(currentObject);
        }
        if (selectedObject != null)
            Destroy(selectedObject.gameObject);
    }
}