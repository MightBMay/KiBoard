using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;
using static MidiReadFile;

public static class MidiDataHandler
{

    // Use this method to retrieve stored MIDI data
    public static NoteEventDataWrapper GetJSONData(string fileName)
    {
        // Load the previously stored data
        // Return the loaded data
        return LoadNoteEventData(fileName);
    }

    public static NoteEventDataWrapper SaveNoteEventData(string fileName, float bpm, List<NoteEventInfo> dataToSave)
    {
        if(dataToSave == null) { Debug.LogError("Data Save Error: NoteEventInfo Null"); return null; }
        // Create a wrapper class to hold both BPM and NoteEventInfo
        var wrapper = new NoteEventDataWrapper
        {
            BPM = bpm,
            NoteEvents = dataToSave
        };

        // Convert the wrapper to a JSON string
        string json = JsonUtility.ToJson(wrapper);

        // Define the path where you want to save the JSON file
        string folderPath = Application.persistentDataPath + "/Songs";
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }


        // Write the JSON string to the file
        File.WriteAllText(folderPath + "/"+fileName+".json", json);
        return wrapper;

    }

    public static NoteEventDataWrapper LoadNoteEventData(string fileName)
    {
        // Define the path from where you want to load the JSON file
        string filePath = Application.persistentDataPath +"/Songs/"+ fileName+".json";

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
[System.Serializable]
public class NoteEventDataWrapper
{
    public float BPM;
    public List<NoteEventInfo> NoteEvents;
}
