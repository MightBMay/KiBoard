using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    string songName = "";
    RawImage image;
    private void Start()
    {
        image = GetComponent<RawImage>();
        SetBackgroundImage();
    }

    public void SetBackgroundImage()
    {

        if (!string.IsNullOrEmpty(GameSettings.currentFileGroup.FileName))
        {
            try { image.texture = GameSettings.currentFileGroup.GetImage("bg_"); }
            catch { Debug.LogWarning("Error assigning background image."); }
        }
        else {
            try { image.texture = GameSettings.currentFileGroup.GetImage("bg_"); }
            catch { Debug.LogWarning("Error assigning background image."); }
        }
    }

    
}

