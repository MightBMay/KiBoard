using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongEditorTutorial : MonoBehaviour
{
    Animator ani;
    bool lockOthers;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { ani.SetTrigger("BeatDenominator"); }
        if (Input.GetMouseButtonDown(1)) { ani.SetTrigger("Continue"); }
    }

}
