using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;

public class MidiDataHandler
{
    /// <summary>
    /// Retrieves stored MIDI data from a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to load.</param>
    /// <returns>The loaded MIDI data as a NoteEventDataWrapper.</returns>
    public static NoteEventDataWrapper GetJSONData(string fileName)
    {
        // Load the previously stored data
        // Return the loaded data
        return LoadNoteEventData(fileName);
    }

    /// <summary>
    /// Saves NoteEventInfo data to a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to save.</param>
    /// <param name="bpm">The BPM (Beats Per Minute) of the song.</param>
    /// <param name="dataToSave">The list of NoteEventInfo to save.</param>
    /// <returns>The wrapper containing BPM and NoteEventInfo.</returns>
    public static NoteEventDataWrapper SaveNoteEventData(string extension, float bpm, List<NoteEventInfo> dataToSave)
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
        string folderPath = GameSettings.currentFileGroup.FolderPath;
        string fileName = Path.GetFileNameWithoutExtension(GameSettings.currentSongPath);
        // Define the path where you want to save the JSON file
        if (!Directory.Exists(folderPath))
        {
            try { Directory.CreateDirectory(folderPath); } catch { }
        }

        // Write the JSON string to the file
        SaveToFileCompressed(json, folderPath + "/" +fileName+ extension);
        return wrapper;
    }
    /// <summary>
    /// ONLY USE FOR REPLAYS UNLESS MODIFIED
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="wrapper"></param>
    /// <returns></returns>
    public static NoteEventDataWrapper SaveNoteEventData(string extension, NoteEventDataWrapper wrapper) 
    {
        if (wrapper == null) { Debug.LogError("Data Save Error: NoteEventDataWrapper Null"); return null; }
        // Create a wrapper class to hold both BPM and NoteEventInfo


        // Convert the wrapper to a JSON string
        string json = JsonUtility.ToJson(wrapper);
        string folderPath = GameSettings.currentFileGroup.FolderPath;
        string fileName = Path.GetFileNameWithoutExtension(GameSettings.currentSongPath);
        // Define the path where you want to save the JSON file
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        int versionCount = 0;
        while (true)
        {
            if (!File.Exists($"{folderPath}/{fileName}_{versionCount}{extension}"))
            {
                SaveToFileCompressed(json, $"{folderPath}/{fileName}_{versionCount}{extension}");
                return wrapper;
            }
            else
            {

                versionCount++;
            }
        }

        // Write the JSON string to the file


    }

    /// <summary>
    /// Loads NoteEventInfo data from a JSON file.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to load.</param>
    /// <returns>The loaded MIDI data as a NoteEventDataWrapper.</returns>
    public static NoteEventDataWrapper LoadNoteEventData(string filePath)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json = LoadFromFileCompressed(filePath);

            // Convert the JSON string back to a hashset of NoteEventInfo
            return JsonUtility.FromJson<NoteEventDataWrapper>(json);
        }
        else
        {
            Debug.LogWarning("Note event data file does not exist.");
            return null;
        }
    }

    /// <summary>
    /// Saves json string "Data" to a compressed json file at "filePath".
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filePath"></param>
    private static void SaveToFileCompressed(string data, string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Compress))
            {
                using (StreamWriter writer = new StreamWriter(zipStream))
                {
                    writer.Write(data);
                }
            }
        }
    }

    /// <summary>
    /// loads a json string from a compressed json file at "filePath".
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>decomressed Json String</returns>
    private static string LoadFromFileCompressed(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                using (StreamReader reader = new StreamReader(zipStream))
                {
                    return reader.ReadToEnd();
                }
            }
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

    /// <summary>
    /// Adds new note to <see cref="NoteEvents"/>.
    /// </summary>
    /// <param name="noteNumber">Note number of the note.</param>
    /// <param name="startTime">Start time of the note.</param>
    /// <param name="endTime">end time of the note.</param>
    public void AddNewNote(int noteNumber, float startTime, float endTime)
    {
        NoteEvents.Add(new NoteEventInfo(noteNumber, startTime, endTime));
    }
    /// <summary>
    /// Clears noteEvents list and sets BPM to negative infinity.
    /// </summary>
    public void Reset()
    {
        BPM = Mathf.NegativeInfinity;
        NoteEvents.Clear();
    }
}
