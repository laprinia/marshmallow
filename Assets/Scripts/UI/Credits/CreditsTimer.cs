using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsTimer : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private float secondsToWait = 10.0f;

    void Start()
    {
        StartCoroutine(DelayedLoadAsyncScene(secondsToWait));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator DelayedLoadAsyncScene(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        StartCoroutine(LoadAsyncScene());
    }
}