using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    float _loadingStartTime;
    public float MinLoadingTime = 1.5f; 

    public void Skip()
    {
        StartCoroutine(LoadGameAsync());
    }

    IEnumerator LoadGameAsync()
    {
        _loadingStartTime = Time.time;

        // Start an asynchronous operation to load the scene
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);

        // Don't lead the scene start until all Studio Banks have finished loading
        async.allowSceneActivation = false;


        while (Time.time < _loadingStartTime + MinLoadingTime)
        {
            yield return null;
        }

        // Allow the scene to be activated. This means that any OnActivated() or Start()
        // methods will be guaranteed that all FMOD Studio loading will be completed and
        // there will be no delay in starting events
        async.allowSceneActivation = true;

        // Keep yielding the co-routine until scene loading and activation is done.
        while (!async.isDone)
        {
            yield return null;
        }

    }

}
