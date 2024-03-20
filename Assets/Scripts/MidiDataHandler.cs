using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;
using static MidiReadFile;

/// <summary>
/// Handles saving and loading MIDI data.
/// </summary>
public static class MidiDataHandler
{
    /// <summary>
    /// Retrieves stored MIDI data from a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to load.</param>
    /// <returns>The loaded MIDI data as a NoteEventDataWrapper.</returns>
    public static NoteEventDataWrapper GetJSONData(string fileName, string extension)
    {
        // Load the previously stored data
        // Return the loaded data
        return LoadNoteEventData(fileName, extension);
    }

    /// <summary>
    /// Saves NoteEventInfo data to a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to save.</param>
    /// <param name="bpm">The BPM (Beats Per Minute) of the song.</param>
    /// <param name="dataToSave">The list of NoteEventInfo to save.</param>
    /// <returns>The wrapper containing BPM and NoteEventInfo.</returns>
    public static NoteEventDataWrapper SaveNoteEventData(string fileName, string extension, float bpm, List<NoteEventInfo> dataToSave)
    {
        if (dataToSave == null) { Debug.LogError("Data Save Error: NoteEventInfo Null"); return null; }
        // Create a wrapper class to hold both BPM and NoteEventInfo
        var wrapper = new NoteEventDataWrapper
        {
            BPM = bpm,
            NoteEvents = dataToSave
        };

        // Convert the wrapper to a JSON string
        string json = JsonUtility.ToJson(wrapper);
        string folderPath;
        // Define the path where you want to save the JSON file
        if (extension.Equals(".replay")) { folderPath = Application.persistentDataPath + "/Replays/"; }
        else { folderPath = Application.persistentDataPath + "/Songs/"; }
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Write the JSON string to the file
        File.WriteAllText(folderPath + fileName + extension, json);
        return wrapper;
    }
    public static NoteEventDataWrapper SaveNoteEventData(string fileName, string extension, NoteEventDataWrapper wrapper)
    {
        if (wrapper == null) { Debug.LogError("Data Save Error: NoteEventDataWrapper Null"); return null; }
        // Create a wrapper class to hold both BPM and NoteEventInfo

        // Convert the wrapper to a JSON string
        string json = JsonUtility.ToJson(wrapper);
        string folderPath;
        // Define the path where you want to save the JSON file
        if (extension.Equals(".replay")) { folderPath = Application.persistentDataPath + "/Replays/"; }
        else { folderPath = Application.persistentDataPath + "/Songs/"; }
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Write the JSON string to the file
        File.WriteAllText(folderPath + fileName + extension, json);
        return wrapper;
    }

    /// <summary>
    /// Loads NoteEventInfo data from a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to load.</param>
    /// <returns>The loaded MIDI data as a NoteEventDataWrapper.</returns>
    public static NoteEventDataWrapper LoadNoteEventData(string fileName, string extension)
    {
        // Define the path from where you want to load the JSON file
        string filePath = Application.persistentDataPath + "/Songs/" + fileName + extension;
        if (extension.Equals(".replay")) { filePath = Application.persistentDataPath + "/Replays/" + fileName + extension; }

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(filePath);

            // Convert the JSON string back to a list of NoteEventInfo
            return JsonUtility.FromJson<NoteEventDataWrapper>(json);
        }
        else
        {
            Debug.LogWarning("Note event data file does not exist.");
            return new NoteEventDataWrapper();
        }
    }
}

/// <summary>
/// Wrapper class for storing BPM and NoteEventInfo data.
/// </summary>
[System.Serializable]
public class NoteEventDataWrapper
{
    /// <summary>
    /// The BPM (Beats Per Minute) of the song.
    /// </summary>
    public float BPM;

    /// <summary>
    /// The list of NoteEventInfo representing MIDI notes.
    /// </summary>
    public List<NoteEventInfo> NoteEvents;

    public void AddNewNote(int noteNumber, float startTime, float endTime)
    {
        NoteEvents.Add(new NoteEventInfo(noteNumber, startTime, endTime));
    }
    public void Reset()
    {
        BPM = Mathf.NegativeInfinity;
        NoteEvents.Clear();
    }
}
