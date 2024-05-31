using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(RawImage))]
public class RuntimeImageLoader : MonoBehaviour
{
    /// <summary>
    /// set true if it the image is song specific ( Will look in the selected song's folder for the image instad of KiBoard/Images/.
    /// </summary>
    [SerializeField] bool isSongSpecific;
    /// <summary>
    /// Prefix for images in the Kiboard/Images folder. Leave blank if you want the image to be song specific.
    /// </summary>

    [SerializeField] string imagePrefix = "";
    RawImage image;
    private void Start()
    {
        image = GetComponent<RawImage>();
        SetImageFromPrefix();
    }
    /// <summary>
    /// Sets rawImage to the image in Kiboard/Images/ with the prefix "imagePrefix
    /// </summary>
    public void SetImageFromPrefix()
    {

        if (isSongSpecific && !string.IsNullOrEmpty(GameSettings.currentFileGroup.FileName))
        {
            try { image.texture = GameSettings.currentFileGroup.GetImage(imagePrefix); }
            catch { Debug.LogWarning($"Error assigning image with prefix: {imagePrefix}."); }
        }
        else
        {
            if (string.IsNullOrEmpty(imagePrefix)) { return; }
            try { image.texture = Utility.GetImageFromImageFolder(imagePrefix); }
            catch { Debug.LogWarning($"Error assigning image with prefix: {imagePrefix}."); }
        }
    }


}
