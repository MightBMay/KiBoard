using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Replay : MonoBehaviour
{
    /// <summary>
    /// singleton reference.
    /// </summary>
    public static Replay instance;
    /// <summary>
    /// Is the currently playing song a .replay file?
    /// </summary>
    public static bool isPlayingReplay = false;
    /// <summary>
    /// Should player input be recorded for a .replay file to be made?
    /// </summary>
    public static bool recordReplay = false;
    /// <summary>
    /// Stores all player input notes to later be saved as a .replay file.
    /// </summary>
    public NoteEventDataWrapper replayNoteData;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    /// <summary>
    /// Clears current replay data, Checks if the selected song is a .replay file and assigns isPlaying Replay, then assigns a BPM to the replay.
    /// </summary>
    public static void StartReplayCapture()
    {
        ClearReplay();
        if (Path.GetExtension(GameSettings.currentSongPath).Equals(".replay")) { isPlayingReplay = true; }
        instance.replayNoteData.BPM = GameSettings.bpm;
    }

    /// <summary>
    /// Adds player input note to the replay data being stored.
    /// </summary>
    /// <param name="noteNum"> number of the note pressed ( 0 - 87) </param>
    /// <param name="time"> what songTime the note was pressed.</param>
    public static void UpdateReplay(int noteNum, float time)
    {
        if (isPlayingReplay|| !recordReplay||time <0) { return; }
        // Check if the note exists with endTime as Mathf.NegativeInfinity
        var existingNote = instance.replayNoteData.NoteEvents.Find(note => note.noteNumber == noteNum && note.endTime == Mathf.NegativeInfinity);
        
        if (existingNote != null)
        {
            // If the note exists, update its endTime
            existingNote.endTime = time;
        }
        else
        {
            // If the note doesn't exist, add a new note with endTime as Mathf.NegativeInfinity
            instance.replayNoteData.AddNewNote(noteNum, time, Mathf.NegativeInfinity);
        }
    }

    /// <summary>
    /// resets replayNoteData to empty values.
    /// </summary>
    public static void ClearReplay()
    {
        instance.replayNoteData.Reset();
    }

    


}