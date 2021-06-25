using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool isInRange;
    [SerializeField] private UnityEvent events;
    [SerializeField] private UnityEvent autoStartEvents;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [SerializeField][ReadOnly] private bool autoEventsDone = false;

    void Update()
    {
        if(isInRange)
        {
            if(Input.GetKeyDown(interactKey))
            {
                events.Invoke();
            }

            if (!autoEventsDone)
            {
                autoStartEvents.Invoke();
                autoEventsDone = true;
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
