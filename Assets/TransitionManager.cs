using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;
    [SerializeField] Animator animator; // Made private and readonly
    static float TransitionDuration = 0.5f; // Made property


    private void Awake()
    {
        if(instance== null)
        {
            instance = this;
        }
        else { Destroy(instance); }
    }

    public void LoadNewScene(int sceneID)
    {
        StartCoroutine(TransitionToScene(sceneID));

    }
    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));

    }

    IEnumerator TransitionToScene(int sceneID)
    {

        animator.SetTrigger("StartFadeOut");
        yield return new WaitForSeconds(1+TransitionDuration);
        SceneManager.LoadScene(sceneID);
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        animator.SetTrigger("StartFadeOut");
        yield return new WaitForSeconds(1+TransitionDuration);
        SceneManager.LoadScene(sceneName);
    }



}