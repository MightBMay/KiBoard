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
    public string[] ImageFiles;
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
        ImageFiles = null;
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
    /// <summary>
    /// gets an image based on prefix from the <see cref="ImageFiles"/> list.
    /// </summary>
    /// <param name="prefix">prefix to search for to differentiate from different images for a song ( eg: icon_songName or bg_songName)</param>
    /// <returns>texture with that image.</returns>
    public Texture2D GetImage(string prefix)
    {
        foreach(string path in ImageFiles)
        {
            string filename = Path.GetFileName(path);
            if (filename.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)){
                return Utility.LoadImageFromFile(path);
            }
        }
        try
        {
            Debug.Log("no "+ prefix +" file found for song "+FileName+". defaulting to 1st png.");
            return Utility.LoadImageFromFile(ImageFiles[0]);
        }
        catch {
            Debug.Log("\"+ prefix +\" Image not found for song " + FileName);
            return null; 
        }
    }

 
}
/// <summary>
/// struct containing information on which files are or aren't located when assembling a file group for a song.
/// </summary>
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
