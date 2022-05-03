using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649, 0414

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorFunctions;
    [SerializeField] int thisIndex;
    public static bool isContinue;

    void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("pressed", true);
                ButtonEvent(thisIndex);
            }
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else
        {
            animator.SetBool("selected", false);
        }
    }

    public void ButtonEvent(int index)
    {
        if (index == 0)
            StartCoroutine(LoadAsyncMainScene());
        else if (index == 1)
        {
            isContinue = true;
            StartCoroutine(LoadAsyncMainScene());
        }
        else if (index == 2)
        {
            StartCoroutine(LoadAsyncMultiScene());
        }
        
    }
    
    
    
    IEnumerator LoadAsyncMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");
        

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadAsyncMultiScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LobbyMenu");
        

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}