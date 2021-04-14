using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent("SaveableCharacter") != null)
        {
            collider.gameObject.GetComponent<SaveableCharacter>().SaveCharacter();
        }
    }
}