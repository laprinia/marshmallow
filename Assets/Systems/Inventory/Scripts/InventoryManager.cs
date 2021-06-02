using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryObject _inventoryObject;
    [SerializeField] private GameObject katana;
    private void Awake()
    {
        _inventoryObject.AddItem(katana.GetComponent<Item>().itemObject, 1);
    }

}
