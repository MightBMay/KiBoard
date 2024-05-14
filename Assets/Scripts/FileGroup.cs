using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Represents a group of files related to a specific song.
/// </summary>
[System.Serializable]
public struct FileGroup
{
    public FileGroupError? errors;
    public string FileName;
    public string FolderPath;
    public string Mp3File;
    public string[] MidiFiles;
    public string[] JsonFiles;
    public string[] PngFiles;
    public string ScoreFile;
    public string[] ReplayFiles;

    /// <summary>
    /// Initializes a new instance of the FileGroup struct with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file group.</param>
    public FileGroup(string fileName)
    {
        errors = null;
        FolderPath = string.Empty;
        FileName = fileName;
        Mp3File = string.Empty;
        MidiFiles = null;
        JsonFiles = null;
        PngFiles = null;
        ScoreFile = string.Empty;
        ReplayFiles = null;
    }

    /// <summary>
    /// Checks the contents of the file group and returns any missing files as a formatted string.
    /// </summary>
    /// <returns>A formatted string indicating missing files.</returns>
    public string CheckFileGroupContents()
    {
        string str = "";
        if (string.IsNullOrEmpty(Mp3File)) { str += "<color=red> MP3 file not found. \n</color>"; }

        if (MidiFiles.Length <= 0)
        {
            if (JsonFiles.Length <= 0)
            {
                str += "<color=red> Midi AND Json file not found. \n</color>";
            }
            else
            {
                str += "<color=red> Midi file not found. \n</color>";
            }
        }


        return str;
    }

    public Texture2D GetIcon()
    {
        foreach(string path in PngFiles)
        {
            string filename = Path.GetFileName(path);
            if (filename.StartsWith("icon_", StringComparison.InvariantCultureIgnoreCase)){
                return LoadImageFromFile(path);
            }
        }
        try
        {
            Debug.Log("no icon_ file found for song "+FileName+", Icon defaulting to 1st png.");
            return LoadImageFromFile(PngFiles[0]);
        }
        catch {
            Debug.Log("Song Icon PNG/JPG not found for song "+FileName);
            return null; 
        }
    }

    public Texture2D GetBackground()
    {
        foreach (string path in PngFiles)
        {
            string filename = Path.GetFileName(path);
            Debug.Log(filename);
            if (filename.StartsWith("bg_", StringComparison.InvariantCultureIgnoreCase)){
                return LoadImageFromFile(path);
            }
        }
            Debug.Log("no bg_ file found for song "+FileName+". Defaulting.");
            return null;
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
[System.Serializable]
public struct FileGroupError
{
    public bool json;
    public bool mp3;
    public bool png;
    public bool midi;
    public bool score;
    public bool replay;

}
