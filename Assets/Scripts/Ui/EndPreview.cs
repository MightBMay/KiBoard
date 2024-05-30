using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPreview : MonoBehaviour
{
    public void EndPreviewFade()
    {
        GetComponent<Animator>().SetTrigger("EndPreview");
    }

    public void ReplayPreview()
    {
        MidiInput.instance.LoadSongFromCurrentSettings(true);
    }
}
