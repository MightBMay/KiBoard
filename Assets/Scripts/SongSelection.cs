using JetBrains.Annotations;
using NAudio.Midi;
using NLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelection : MonoBehaviour
{
    public static SongSelection instance;
    public Animator animator;

    /// <summary>
    /// Contains groups of midi,mp3,png, and json files that share the same file name.
    /// </summary>
    [SerializeField] List<FileGroup> fileGroups;

    /// <summary>
    /// Reference to the start song button..
    /// </summary>
    public Button startButton;

    string defaultPath;


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
        defaultPath = Application.persistentDataPath + "/Songs/";
        // Assign the song directory path to a local variable.
        if (Directory.Exists(defaultPath))
        {
            string[] directoryPath = Directory.GetDirectories(defaultPath);
            foreach (string directory in directoryPath)
            {
                fileGroups.Add(AssembleFileGroup(directory));
            }
        }
        else
        { // if directory not found, Log the error, create the directory, and continue.
            Debug.LogError("Directory not found: " + defaultPath + ". Creating Directory.");
            Directory.CreateDirectory(defaultPath);
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
    public static string GetUnderscoreSubstring(string input)
    {

        if (string.IsNullOrEmpty(input)) return input;// Find the index of the first underscore character in the input string
        int underscoreIndex = input.IndexOf('_');


        // Return a substring from the start of the input string to the underscore index, inclusive
        return (underscoreIndex >= 0) ? input.Substring(0, underscoreIndex) : input;
    }

    public static string GetPostUnderscoreSubstring(string input)
    {
        // Find the index of the first underscore character in the input string
        int underscoreIndex = input.IndexOf('_');

        // Return a substring from the start of the input string to the underscore index, inclusive
        return (underscoreIndex >= 0) ? input.Substring(underscoreIndex + 1) : input;
    }

    /// <summary>
    /// Assembles a FileGroup object based on the given file name.
    /// </summary>

    public FileGroup AssembleFileGroup(string directory)
    {
        string[] allFiles = Directory.GetFiles(directory);
        FileGroupError error = new FileGroupError();
        string[] jsonfiles = null, pngfiles = null, midifiles = null, replayfiles = null;
        string mp3file = "", scorefile = "";
        try { jsonfiles = allFiles.Where(file => Path.GetExtension(file) == ".json").ToArray(); error.json = true; } catch { }
        try { mp3file = allFiles.First(file => Path.GetExtension(file) == ".mp3"); error.mp3 = true; } catch { }
        try { pngfiles = allFiles.Where(file => Path.GetExtension(file) == ".png").ToArray(); error.png = true; } catch { }
        try { midifiles = allFiles.Where(file => Path.GetExtension(file) == ".mid").ToArray(); error.midi = true; } catch { }
        try { scorefile = allFiles.First(file => Path.GetExtension(file) == ".score"); error.score = true; } catch { }
        try { replayfiles = allFiles.Where(file => Path.GetExtension(file) == ".replay").ToArray(); error.replay = true; } catch { } // don't need to do anything in catch since the values for the error struct are automatically false.
        return new FileGroup()
        {
            errors = error,
            FileName = Path.GetFileNameWithoutExtension(directory),
            FolderPath = directory,
            JsonFiles = jsonfiles,
            Mp3File = mp3file,
            PngFiles = pngfiles,
            ScoreFile = scorefile,
            ReplayFiles = replayfiles,
            MidiFiles = midifiles,

        };
    }

    /// <summary>
    /// Starts the game if the current song name is not null or empty.
    /// </summary>
    public void StartGame()
    {
        // Check if the current song name is null or empty, and return if true
        if (string.IsNullOrEmpty(GameSettings.currentSongPath)) { return; }
        TransitionManager.canTransition = true;
        // Load the song from current game settings using the GameManager
        MidiInput.instance.LoadSongFromCurrentSettings(false);

    }

}
