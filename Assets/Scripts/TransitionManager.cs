using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;
    public static bool canTransition;
    [SerializeField] Animator animator; // Made private and readonly
    static float TransitionDuration = 0.25f; // Made property


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(instance); }
    }

    public void LoadNewScene(int sceneID)
    {
        if (GameManager.instance != null) { GameManager.instance.startTimer = false; }
        StartCoroutine(TransitionToScene(sceneID));

    }
    public void LoadNewScene(string sceneName)
    {
        if (GameManager.instance != null) { GameManager.instance.startTimer = false; }
        StartCoroutine(TransitionToScene(sceneName));

    }

    IEnumerator TransitionToScene(int sceneID)
    {

        if (canTransition) animator.SetTrigger("StartFadeOut");
        yield return new WaitForSeconds(1 + TransitionDuration);
        SceneManager.LoadScene(sceneID);
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        if (canTransition) animator.SetTrigger("StartFadeOut");
        yield return new WaitForSeconds(1 + TransitionDuration);
        SceneManager.LoadScene(sceneName);
    }



}
