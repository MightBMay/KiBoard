using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;


public class Utility : MonoBehaviour
{
    /// <summary>
    /// image file extensions supported.
    /// </summary>
    public static string[] SupportedImageTypes = new[] { ".png", ".jpg", ".jpeg" };// ".gif",".bmp",".tif",".tiff",".tga"};


    /// <summary>
    /// Array of all directories in the ".../Kiboard/Images/" folder.
    /// </summary>
    private static string[] ImagesDirectories;

    /// <summary>
    /// public reference to the array of all directories in the ".../Kiboard/Images/" folder.
    /// </summary>
    public static string[] imagesDirectories
    {
        get
        {
            if (ImagesDirectories == null)
            {
                ImagesDirectories = Directory.GetFiles(Application.persistentDataPath + "/Images/");
            }
            return ImagesDirectories;
        }
        set
        {
            ImagesDirectories = value;
        }
    }



    /// <summary>
    /// normalizes number Input to a range min to max.
    /// </summary>
    public static float NormalizeNumberToRange(float input, float min, float max)
    {
        return min + (input * (max - min));
    }

    

    /// <summary>
    /// loads an image from given path to a Texture2D.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
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

    /// <summary>
    /// gets an image based on prefix from the "<see cref="Application.persistentDataPath"/>/Images/" folder.
    /// </summary>
    /// <param name="prefix">prefix to search for to differentiate from different images based on scene.( eg: SceneSelection_bg, GameScene88_bg)</param>
    /// <returns>texture with that image.</returns>
    public static Texture2D GetImageFromImageFolder(string prefix)
    {
        foreach (string path in imagesDirectories)
        {
            if (Path.GetFileName(path).StartsWith(prefix, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return LoadImageFromFile(path);
            }
        };

        Debug.LogWarning("no file with prefix " + prefix + " found in Images folder.");
        return null; 

    }
    






}

