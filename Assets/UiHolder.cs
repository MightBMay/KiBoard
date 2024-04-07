using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHolder : MonoBehaviour
{
    public static UiHolder instance;
    public Animator animator;
    public GameObject scenePreview;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }


    public void OnOpenGameSettings()
    {
        animator.SetBool("isSongSelected", false);
    }

    public void OnOpenPlayerSettings()
    {
        animator.SetBool("isSongSelected", false);
    }

    public void OnOpenSongSelect()
    {
        animator.SetBool("isSongSelected", true);
    }
}
