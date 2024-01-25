using System.IO;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a group of files related to a specific song.
/// </summary>
[System.Serializable]
public struct FileGroup
{
    public string FileName;
    public string Mp3File;
    public List<string> MidiFiles;
    public List<string> JsonFiles;
    public string PngFile;

    /// <summary>
    /// Initializes a new instance of the FileGroup struct with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file group.</param>
    public FileGroup(string fileName)
    {
        FileName = fileName;
        Mp3File = string.Empty;
        MidiFiles = new List<string>();
        JsonFiles = new List<string>();
        PngFile = string.Empty;
    }

    /// <summary>
    /// Checks the contents of the file group and returns any missing files as a formatted string.
    /// </summary>
    /// <returns>A formatted string indicating missing files.</returns>
    public string CheckFileGroupContents()
    {
        string str = "";
        if (string.IsNullOrEmpty(Mp3File)) { str += "<color=red> MP3 file not found. \n</color>"; }

        if (MidiFiles.Count <= 0)
        {
            if (JsonFiles.Count <= 0)
            {
                str += "<color=red> Midi AND Json file not found. \n</color>";
            }
            else
            {
                str += "<color=red> Midi file not found. \n</color>";
            }
        }
        if (string.IsNullOrEmpty(PngFile)) { str += "<color=red> Song Icon PNG not found. \n</color>"; }


        return str;
    }

    /// <summary>
    /// Loads an image from the specified file path and returns it as a Texture2D.
    /// </summary>
    /// <param name="path">The file path of the image.</param>
    /// <returns>The loaded image as a Texture2D.</returns>
    public Texture2D LoadImageFromFile(string path)
    {
        if (File.Exists(path))
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
            return null;
        }
    }
}
