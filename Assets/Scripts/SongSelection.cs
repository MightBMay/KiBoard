using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelection : MonoBehaviour
{
    public static SongSelection instance;

    /// <summary>
    /// Contains groups of midi,mp3,png, and json files that share the same file name.
    /// </summary>
    [SerializeField] List<FileGroup> fileGroups;
    /// <summary>
    /// dictionary of all MP3 files at Application.persistntdatapath.
    /// </summary>
    public Dictionary<string, string> mp3Files;
    /// <summary>
    /// dictionary of all MP3 files at Application.persistntdatapath.
    /// </summary>
    public Dictionary<string, List<string>> midiFiles;
    /// <summary>
    /// dictionary of all Midi files at Application.persistntdatapath.
    /// </summary>
    public Dictionary<string, string> pngFiles;
    /// <summary>
    /// dictionary of all PNG files at Application.persistntdatapath.
    /// </summary>
    public Dictionary<string, List<string>> jsonFiles;
    /// <summary>
    /// Reference to the start song button..
    /// </summary>
    public Button startButton;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    void Start()
    {
        // Assign the song directory path to a local variable.
        string directoryPath = Application.persistentDataPath + "/Songs/"; 

        // label to goto if path is created after it isn't found. 
        CreateSongsFolder:

        //Check if path exists.
        if (Directory.Exists(directoryPath))
        { 

            // Get an array of all file paths in the directory
            string[] allFiles = Directory.GetFiles(directoryPath);
            // Separate files based on their extensions
            mp3Files = GetFilesByExtension(allFiles, ".mp3");
            midiFiles = GetListFilesByExtension(allFiles, ".mid");
            pngFiles = GetFilesByExtension(allFiles, ".png");
            jsonFiles = GetListFilesByExtension(allFiles, ".json");

            // create empty string list that will contain one filename. ( this realistically should be a dictionary or hashset but oh well).
            List<string> fileNames = new List<string>();
            foreach (string s in allFiles)
            {
                string filename = GetUnderscoreSubstring(Path.GetFileNameWithoutExtension(s)); // gets only the file name
                if (fileNames.Contains(filename)) { continue; } // if the filename is already in the list, go to the next iteration of the loop.


                fileNames.Add(filename); // if filename is not in list, add it to the list.
                fileGroups.Add(AssembleFileGroups(filename));// assemble a filegroup with the filename, and add it to the fileGroups list.
            }



        }
        else
        { // if directory not found, Log the error, create the directory, and continue.
            Debug.LogError("Directory not found: " + directoryPath);
            Directory.CreateDirectory(directoryPath);
            goto CreateSongsFolder;
        }
        FindObjectOfType<SongList>().SpawnSongItems(fileGroups); // spawns the UI elements based off of the newly assembled fileGroups list.
    }


    /// <summary>
    /// Retrieves a dictionary where keys are file names without extensions and values are full file names with a specified extension.
    /// </summary>
    private Dictionary<string, string> GetFilesByExtension(string[] files, string extension)
    {
        // Initialize a dictionary to store file names without extensions as keys and full file names as values
        Dictionary<string, string> tempDictionary = new();

        // Filter the input array of file names to include only those with the specified extension
        string[] allOfExtension = Array.FindAll(files, file => Path.GetExtension(file).Equals(extension, System.StringComparison.OrdinalIgnoreCase));

        // Iterate through the filtered file names
        foreach (string s in allOfExtension)
        {
            // Extract the file name without extension
            string filename = Path.GetFileNameWithoutExtension(s);

            // Check if the file name without extension is not already a key in the dictionary
            if (!tempDictionary.ContainsKey(filename))
            {
                // Add the file name without extension as key and the full file name as value to the dictionary
                tempDictionary.Add(filename, s);
            }
        }
        return tempDictionary;
    }

    /// <summary>
    /// Retrieves a dictionary where keys are file names without extensions and values are lists of full file names with a specified extension.
    /// </summary>
    private Dictionary<string, List<string>> GetListFilesByExtension(string[] files, string extension)
    {
        // Initialize a dictionary to store file names without extensions as keys and lists of full file names as values
        Dictionary<string, List<string>> tempDictionary = new Dictionary<string, List<string>>();

        // Filter the input array of file names to include only those with the specified extension
        string[] allOfExtension = Array.FindAll(files, file => Path.GetExtension(file).Equals(extension, System.StringComparison.OrdinalIgnoreCase));

        // Iterate through the filtered file names
        foreach (string filePath in allOfExtension)
        {
            // Extract the file name without extension and modify it
            string fileName = GetUnderscoreSubstring(Path.GetFileNameWithoutExtension(filePath));

            // Try to retrieve the list of full file names associated with the modified file name
            if (!tempDictionary.TryGetValue(fileName, out List<string> fileList))
            {
                // If the list doesn't exist, create a new list and add it to the dictionary
                fileList = new List<string>();
                tempDictionary.Add(fileName, fileList);
            }

            // Add the full file name to the list
            fileList.Add(filePath);
        }

        return tempDictionary;
    }

    /// <summary>
    /// Gets a substring of the input string up to the first underscore character (inclusive).
    /// </summary>
    string GetUnderscoreSubstring(string input)
    {
        // Find the index of the first underscore character in the input string
        int underscoreIndex = input.IndexOf('_');

        // Return a substring from the start of the input string to the underscore index, inclusive
        return (underscoreIndex >= 0) ? input.Substring(0, underscoreIndex) : input;
    }

    /// <summary>
    /// Assembles a FileGroup object based on the given file name.
    /// </summary>
    private FileGroup AssembleFileGroups(string fileName)
    {
        // Create a new FileGroup object with the given file name
        FileGroup group = new(fileName);

        // Try to retrieve associated files for different file types and assign them to the FileGroup object
        if (jsonFiles.TryGetValue(fileName, out List<string> jsonFile))
        {
            group.JsonFiles = jsonFile;
        }

        if (pngFiles.TryGetValue(fileName, out string pngFile))
        {
            group.PngFile = pngFile;
        }

        if (mp3Files.TryGetValue(fileName, out string mp3File))
        {
            group.Mp3File = mp3File;
        }

        if (midiFiles.TryGetValue(fileName, out List<string> midiFile))
        {
            group.MidiFiles = midiFile;
        }
        return group;
    }

    /// <summary>
    /// Starts the game if the current song name is not null or empty.
    /// </summary>
    public void StartGame()
    {
        // Check if the current song name is null or empty, and return if true
        if (string.IsNullOrEmpty(SettingsManager.instance.gameSettings.currentSongName)) { return; }
        if (Input.GetKey(KeyCode.LeftShift)) { GameManager.instance.EnterSongEditor(); } // if you hold shift and start, enter the editor.
        else
        {
            // Load the song from current game settings using the GameManager
            GameManager.instance.LoadSongFromCurrentGameSettings();
        }
    }

}
