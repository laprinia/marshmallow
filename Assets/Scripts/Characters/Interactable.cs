using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649

public class Interactable : MonoBehaviour
{
    [Header("NPC Settings")][Space(5)]
    [SerializeField] private NPCController controller;
    [SerializeField] private bool isInRange;
    [SerializeField] private UnityEvent additionalEvents;
    [SerializeField][ReadOnly] private KeyCode interactKey = KeyCode.E;

    void Update()
    {
        if(isInRange)
        {
            if(Input.GetKeyDown(interactKey))
            {
                controller.TriggerLogic();
                additionalEvents.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
