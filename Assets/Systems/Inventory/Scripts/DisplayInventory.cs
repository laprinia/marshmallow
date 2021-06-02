using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    private const int MAX_COUNT = 12;
    //TODO ADD A PLAYER SCRIPT
    public GameObject player;
    public InventoryObject inventory;
    public float xStart = 208.7f;
    public float yStart = 417.6f;
    public int xOffset;

    public int noColumns;

    public int yOffset;
    Dictionary<InventorySlot, GameObject> itemDisplayed = new Dictionary<InventorySlot, GameObject>();

    public void binItemByObj(GameObject obj)
    {
        InventorySlot slot = itemDisplayed.FirstOrDefault(x => x.Value == obj).Key;

        if (slot.item.itemType.Equals(ItemType.Consumable) || slot.item.itemType.Equals(ItemType.Default))
        {
            slot.amount = 0;
            Destroy(obj);
        }

        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }

    void Start()
    {
        CreateDisplay();
    }

    void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            AddToItemsDisplayed(i);
        }
    }

    private void Update()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (itemDisplayed.ContainsKey(inventory.Container[i]) && inventory.Container[i].amount > 0)
            {
                itemDisplayed[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text =
                    inventory.Container[i].amount.ToString("n0");
                
            }
            else if (inventory.Container[i].amount <= 0)
            {
                itemDisplayed.Remove(inventory.Container[i]);
               
               
            }
            else
            {
                
                var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponent<RectTransform>().transform.position = Vector3.zero;
                obj.GetComponent<RectTransform>().transform.localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");

                itemDisplayed.Add(inventory.Container[i], obj);
                
               // AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClick(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnBeginDrag(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            }
        }
    }

    void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    void AddToItemsDisplayed(int indexInInventory)
    {
        int i = indexInInventory;
        var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
        obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
        obj.GetComponent<RectTransform>().transform.position = Vector3.zero;
        obj.GetComponent<RectTransform>().transform.localPosition = GetPosition(i);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
        itemDisplayed.Add(inventory.Container[i], obj);
    }

    // void OnClick(GameObject gameObject)
    // {
    //     InventorySlot slot = itemDisplayed.FirstOrDefault(x => x.Value == gameObject).Key;
    //   
    //     if (slot.item.itemType.Equals(ItemType.Consumable))
    //     {
    //         if (slot.amount > 0)
    //         {
    //             ConsumableObject consumableObject = slot.item as ConsumableObject;
    //
    //             if (slot.item.name.Equals("Blue Flower") && player.GetComponent<Sanity>().curSanity < 100)
    //             {
    //                 player.GetComponent<Sanity>().AddSanity(consumableObject.restorativePower);
    //                 slot.amount--;
    //                 
    //                 
    //             }
    //             else if (slot.item.name.Equals("Daruma") && player.GetComponent<Luck>().curLuck < 100)
    //             {
    //                 player.GetComponent<Luck>().AddLuck(consumableObject.restorativePower);
    //                 slot.amount--;
    //                
    //             }
    //             if(slot.amount<=0){
    //                 
    //                 Destroy(gameObject);
    //             }
    //         }
    //     }
    // }

    void OnBeginDrag(GameObject obj)
    {
        var mouseObj = new GameObject();
        var rt = mouseObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObj.transform.SetParent(transform.parent);
        var img = mouseObj.AddComponent<Image>();
        img.sprite = obj.GetComponent<Image>().sprite;
        img.raycastTarget = false;
        mouseItem.obj = mouseObj;
        InventorySlot slot = itemDisplayed.FirstOrDefault(x => x.Value == gameObject).Key;
        mouseItem.item = slot;
        obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    void OnEndDrag(GameObject obj)
    {
        //if is in bin

        obj.GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }

    void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
    }

    Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (xOffset * (i % noColumns)), yStart + (-yOffset * (i / noColumns)), 0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}