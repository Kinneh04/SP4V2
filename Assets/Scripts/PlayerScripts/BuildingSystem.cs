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
    public Transform currentPreview;

    public InventoryManager im;
    public GameObject woodObj;
    public CreatePopup cp;

    // Variables for raycast
    public Transform cam;
    public RaycastHit hit;
    public LayerMask layer; // To assign it to exclude BuildPreview in raycast checks
    public LayerMask buildableLayer;

    public float offset = 1.0f;
    public float gridSize = 1.0f;

    public bool IsBuilding = false;
    private bool IsChoosingObj = false;
    public float BuildCooldown = 1.0f;
    public GameObject menuObject;

    void Start()
    {
        // Start off with a foundation
        currentObject = objects[0];
        ChangeCurrentBuilding(0);
        // Disable objects until using Building Plan
        menuObject.SetActive(false);
        currentPreview.gameObject.SetActive(false);
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
                        currPreview.position = new Vector3(hit.collider.gameObject.transform.position.x - 0.632f, hit.collider.gameObject.transform.position.y + 0.57f, hit.collider.gameObject.transform.position.z + 1.433f);
                        currPreview.rotation = hit.collider.gameObject.transform.rotation;
                        currentPos = currPreview.position;
                        currentRot = currPreview.rotation.ToEulerAngles();
                        currentPreview.GetComponent<PreviewObject>().IsBuildable = true;
                        return;
                    }
                }
            }
            currentPreview.GetComponent<PreviewObject>().IsBuildable = false;
        }
        else if (currentObject.name == "Floor")
        {
            // Snap to top of walls when looking at them, and next to other floors
        }

        if (Input.GetKeyDown(KeyCode.R))
            currentRot += new Vector3(0, 90, 0);
        currPreview.localEulerAngles = currentRot;
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
