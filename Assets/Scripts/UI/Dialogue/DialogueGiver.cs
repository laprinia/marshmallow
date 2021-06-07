using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGiver : MonoBehaviour
{
    [SerializeField] DialogueManager m_dialogueManager;
    [SerializeField] private DialogueLine[] m_dialogueLines;
    
     private bool sent = false;
    [SerializeField] private bool isAutomatic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!sent)
        {
            sent = true;
            m_dialogueManager.m_dialogueLines = m_dialogueLines;
            m_dialogueManager.isAutomatic = isAutomatic;
        }
    }

   
}
