using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    RawImage image;
    private void Start()
    {
        image = GetComponent<RawImage>();
        SetBackgroundImage();
    }

    public void SetBackgroundImage()
    {

        Texture2D bgTexture = GameSettings.currentFileGroup.GetImage("bg_");
        image.texture = bgTexture;
    }
}

