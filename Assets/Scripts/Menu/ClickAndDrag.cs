using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ClickAndDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform dragRectTransform;
    private string slotname;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 OGPosition;
    public bool canSwap;
    public int Slot;
    public int intSlotToSwapWith;
    GameObject clone;
    public GameObject emptySlot;
    InventoryManager IM;




    private void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        slotname = gameObject.name;
        Slot = GetComponentInChildren<ReadInventory>().SlotNumber;
        IM = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        OGPosition = gameObject.transform.position;
        clone = Instantiate(emptySlot, OGPosition, Quaternion.identity);
        clone.transform.parent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IM.InventoryList[Slot] != null)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            CheckForAvailableSlot();
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.transform.position = OGPosition;
        gameObject.GetComponent<Image>().color = new Color(1f, 1, 1f, 0.05f);
        Destroy(clone);
        if(canSwap && IM.InventoryList[Slot] != null)
        {
            SelectSwappableSlot();
        }

    }

    void SelectSwappableSlot()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            print(raycastResults[i].gameObject.name);
            if (raycastResults[i].gameObject.CompareTag("iSlot") && slotname != raycastResults[i].gameObject.name)
            {
               intSlotToSwapWith = raycastResults[i].gameObject.GetComponentInChildren<ReadInventory>().SlotNumber;

                    IM.SwapTwoSlots(Slot, intSlotToSwapWith);

                

                return;
            }
            else if(raycastResults[i].gameObject.CompareTag("dropArea") && slotname != raycastResults[i].gameObject.name)
            {
                IM.Drop(Slot);
                return;
            }
        }
    }

    public void CheckForAvailableSlot()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        
        for(int i = 0; i < raycastResults.Count; i++)
        {
            print(raycastResults[i].gameObject.name);
            if (raycastResults[i].gameObject.CompareTag("iSlot") && slotname != raycastResults[i].gameObject.name)
            {
                Debug.Log("Dropped on object with tag iSlot");
                gameObject.GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.4f, 0.7f);
                canSwap = true;
                return;
            }

            else if (raycastResults[i].gameObject.CompareTag("dropArea") && slotname != raycastResults[i].gameObject.name)
            {
                Debug.Log("Dropped on object with tag Dslot");
                gameObject.GetComponent<Image>().color = new Color(0.1f, 0.4f, 0.4f, 0.7f);
                canSwap = true;
                return;
            }
        }
        Debug.Log("Moving!");
        canSwap = false;
        gameObject.GetComponent<Image>().color = new Color(1f, 1, 1f, 0.05f);

    }
}
