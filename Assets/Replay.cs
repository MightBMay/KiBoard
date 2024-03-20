using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Replay : MonoBehaviour
{
    public static Replay instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    public NoteEventDataWrapper replayNoteEvents;
    public static void StartReplayCapture()
    {
        ClearReplay();
        instance.replayNoteEvents.BPM = GameSettings.bpm;
    }

    public static void UpdateReplay(int noteNum, float time)
    {
        // Check if the note exists with endTime as Mathf.NegativeInfinity
        var existingNote = instance.replayNoteEvents.NoteEvents.Find(note => note.noteNumber == noteNum && note.endTime == Mathf.NegativeInfinity);

        if (existingNote != null)
        {
            // If the note exists, update its endTime
            existingNote.endTime = time;
        }
        else
        {
            // If the note doesn't exist, add a new note with endTime as Mathf.NegativeInfinity
            instance.replayNoteEvents.AddNewNote(noteNum, time, Mathf.NegativeInfinity);
        }
    }

    public static void ClearReplay()
    {
        instance.replayNoteEvents.Reset();
    }


}