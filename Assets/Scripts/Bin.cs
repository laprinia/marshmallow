using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bin : MonoBehaviour,IDropHandler
{
    public GameObject inventory;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag)
        {
            inventory.GetComponent<DisplayInventory>().binItemByObj(eventData.pointerDrag);
        }
    }
}