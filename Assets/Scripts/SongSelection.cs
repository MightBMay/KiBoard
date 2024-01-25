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
    [SerializeField] List<FileGroup> fileGroups;
    public Dictionary<string, string> mp3Files;
    public Dictionary<string, List<string>> midiFiles;
    public Dictionary<string, string> pngFiles;
    public Dictionary<string, List<string>> jsonFiles;
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
        string directoryPath = Application.persistentDataPath + "/Songs/";

        // Check if the directory exists
        if (Directory.Exists(directoryPath))
        {
            // Get an array of all file paths in the directory
            string[] allFiles = Directory.GetFiles(directoryPath);
            // Separate files based on their extensions
            mp3Files = GetFilesByExtension(allFiles, ".mp3");
            midiFiles = GetListFilesByExtension(allFiles, ".mid");
            pngFiles = GetFilesByExtension(allFiles, ".png");
            jsonFiles = GetListFilesByExtension(allFiles, ".json");

            List<string> fileNames = new List<string>();
            foreach (string s in allFiles)
            {
                string filename = GetUnderscoreSubstring(Path.GetFileNameWithoutExtension(s));
                if (fileNames.Contains(filename)) { continue; }


                fileNames.Add(filename);
                fileGroups.Add(AssembleFileGroups(filename));
            }



        }
        else
        {
            Debug.LogError("Directory not found: " + directoryPath);
        }
        FindObjectOfType<SongList>().SpawnSongItems(fileGroups);
    }



    private Dictionary<string, string> GetFilesByExtension(string[] files, string extension)
    {
        Dictionary<string, string> tempDictionary = new();
        string[] allOfExtension = Array.FindAll(files, file => Path.GetExtension(file).Equals(extension, System.StringComparison.OrdinalIgnoreCase));
        foreach (string s in allOfExtension)
        {
            string filename = Path.GetFileNameWithoutExtension(s);
            if (!tempDictionary.ContainsKey(filename))
            {
                tempDictionary.Add(filename, s);
            }
        }
        return tempDictionary;
    }
    private Dictionary<string, List<string>> GetListFilesByExtension(string[] files, string extension)
    {
        Dictionary<string, List<string>> tempDictionary = new Dictionary<string, List<string>>();
        string[] allOfExtension = Array.FindAll(files, file => Path.GetExtension(file).Equals(extension, System.StringComparison.OrdinalIgnoreCase));
        foreach (string filePath in allOfExtension)
        {
            string fileName = GetUnderscoreSubstring(Path.GetFileNameWithoutExtension(filePath));

            if (!tempDictionary.TryGetValue(fileName, out List<string> fileList))
            {
                fileList = new List<string>();
                tempDictionary.Add(fileName, fileList);
            }

            fileList.Add(filePath);
        }

        return tempDictionary;

    }
    string GetUnderscoreSubstring(string input)
    {
        int underscoreIndex = input.IndexOf('_');
        return (underscoreIndex >= 0) ? input.Substring(0, underscoreIndex) : input;
    }


    private FileGroup AssembleFileGroups(string fileName)
    {
        FileGroup group = new(fileName);
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

    public void StartGame()
    {
        if (string.IsNullOrEmpty(SettingsManager.instance.gameSettings.currentSongName)) { return; }


        GameManager.instance.LoadSongFromCurrentGameSettings();

    }

}

