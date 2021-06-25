using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

public class HamsterController : MonoBehaviour
{
    [SerializeField] private GameObject itemToDestroy;
    public void DeletePepper()
    {
        Destroy(itemToDestroy);
        Destroy(gameObject);
    }
}
