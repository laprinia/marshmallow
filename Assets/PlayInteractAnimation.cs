using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayInteractAnimation : MonoBehaviour
{
    [SerializeField] private float timeToDissapear;
    [SerializeField] private Animator animator;

    private bool isShown;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isShown)
        {
            
            animator.SetTrigger("appear");
            isShown=true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (isShown)
        {
            StartCoroutine(WaitCoroutine());
        }
        
    }

    IEnumerator WaitCoroutine()
    {
        
        yield return new WaitForSeconds(timeToDissapear);
        animator.SetTrigger("disappear");
        yield return new WaitForSeconds(0.4f);
        animator.ResetTrigger("disappear");
        isShown=false;

    }
}
