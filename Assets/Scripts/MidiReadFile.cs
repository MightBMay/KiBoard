using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;


public static class MidiReadFile { 

    static Dictionary<int, NoteEventInfo> activeNotes = new Dictionary<int, NoteEventInfo>();







    public static NoteEventDataWrapper GetNoteEventsFromName(string songName)
    {
        string path = Application.persistentDataPath + "/Songs/" + songName;
        string jsonFilePath = path + ".json";
        string midiFilePath = path + ".mid";
        if (File.Exists(jsonFilePath))
        {
            return GetDataFile(songName);
        }
        else if (File.Exists(midiFilePath))
        {
            return ReadMidiFile(midiFilePath, songName);
        }
        else
        {
            Debug.LogError("NO .JSON/.MID FILE FOUND WITH NAME: " + songName);
            return null;
        }

    }


    public static NoteEventDataWrapper GetDataFile(string jsonFilePath)
    {
        return MidiDataHandler.GetJSONData(jsonFilePath);
    }

    static NoteEventDataWrapper ReadMidiFile(string midiFilePath, string songName)
    {

        MidiFile midiFile = new MidiFile(midiFilePath, false);
        float bpm = 0;
        List<NoteEventInfo> noteEvents = new();
        foreach (var trackChunk in midiFile.Events)
        {
            foreach (var midiEvent in trackChunk)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    ProcessNoteOnEvent((NoteEvent)midiEvent);
                }
                else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                {
                    ProcessNoteOffEvent((NoteEvent)midiEvent);
                }
                else if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                {
                    MetaEvent metaEvent = (MetaEvent)midiEvent;

                    // Check for tempo events
                    if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                    {
                        TempoEvent tempoEvent = (TempoEvent)metaEvent;
                        long microsecondsPerQuarterNote = tempoEvent.MicrosecondsPerQuarterNote;

                        bpm = 60000000f / microsecondsPerQuarterNote;
                        // Process tempo event as needed
                    }
                    // Add more conditions as needed for other meta-event types
                }
            }
        }
        if (bpm == 0) { Debug.LogError("BPM WAS NOT FOUND/ IS 0."); return null; }
        return MidiDataHandler.SaveNoteEventData(songName, bpm, noteEvents);

        void ProcessNoteOnEvent(NoteEvent noteOnEvent)//```````````````````````````````````````````````````````modify so if you (somehow ) press a note 2 times before you let go of the first note, it marks its end time.
        {
            Debug.Log(noteOnEvent.NoteNumber + "" + noteOnEvent.NoteName);
            int noteNumber = noteOnEvent.NoteNumber;

            float ticksPerQuarterNote = 96; // Adjust this based on your DAW
            float secondsPerTick = 60.0f / (bpm * ticksPerQuarterNote);

            // Then, use secondsPerTick in your timing calculations
            float startTime = noteOnEvent.AbsoluteTime * secondsPerTick;

            // Convert MIDI note number to note name
            string noteName = ConvertNoteNumberToName(noteNumber);

            // Create a new NoteEventInfo or update an existing one
            // Create a new NoteEventInfo for the note
            NoteEventInfo noteEventInfo = new NoteEventInfo
            {
                noteNumber = noteNumber,
                startTime = startTime
            };
            if (!activeNotes.ContainsKey(noteNumber))
            {
                activeNotes.Add(noteNumber, noteEventInfo);
            }
            noteEvents.Add(noteEventInfo);
        }

        void ProcessNoteOffEvent(NoteEvent noteOffEvent)
        {
            int noteNumber = noteOffEvent.NoteNumber;

            float ticksPerQuarterNote = 96; // Adjust this based on your DAW
            float secondsPerTick = 60.0f / (bpm * ticksPerQuarterNote);

            // Print the value of noteOffEvent.AbsoluteTime

            // Calculate end time in seconds
            float endTime = noteOffEvent.AbsoluteTime * secondsPerTick;

            // Check if the note is in the activeNotes dictionary
            if (activeNotes.TryGetValue(noteNumber, out NoteEventInfo activeNote))
            {
                activeNote.endTime = endTime;

                // Optionally, add the activeNote to your noteEvents list if you want to keep track of all notes

                // Remove the note from the activeNotes dictionary since it's no longer active
                activeNotes.Remove(noteNumber);
            }
            else
            {

            }
        }



        string ConvertNoteNumberToName(int noteNumber)
        {
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            int octave = (noteNumber / 12);

            int noteIndex = noteNumber % 12;
            string noteName = noteNames[noteIndex];

            return $"{noteName}{octave}";
        }



    }
}


