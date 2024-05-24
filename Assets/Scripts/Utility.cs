using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;


public class Utility : MonoBehaviour
{
    public static string[] SupportedImageTypes = new[] { ".png", ".jpg", ".jpeg" };// ".gif",".bmp",".tif",".tiff",".tga"};
    public static float NormalizeNumberToRange(float input, float min, float max)
    {
        return min + (input * (max - min));
    }

    //
    public static Texture2D LoadImageFromFile(string path)
    {
        if (File.Exists(path) || SupportedImageTypes.Contains(Path.GetExtension(path)))
        {
            // Step 1: Read PNG file
            byte[] fileData = File.ReadAllBytes(path);

            // Step 2: Create Texture2D
            Texture2D texture = new Texture2D(128, 128); // You can adjust the size if needed
            texture.LoadImage(fileData); // This automatically resizes the texture

            // Step 3: Assign to RawImage
            return texture;
        }
        else
        {
            Debug.Log("Error loading image file from path " + path);
            return null;
        }
    }



}

