using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadToSongSelect());
    }
    IEnumerator LoadToSongSelect()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        TransitionManager.instance.LoadNewScene("SongSelect");
    }
}
