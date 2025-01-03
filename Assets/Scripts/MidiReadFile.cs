using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class MidiReadFile
{

    /// <summary>
    /// Dictionary storing what notes are active at a given song time. Used to make sure you cannot press a note twice without releasing it.
    /// </summary>
    static Dictionary<int, NoteEventInfo> activeNotes = new Dictionary<int, NoteEventInfo>();
    /// <summary>
    /// Loads <see cref="NoteEventDataWrapper"/> from a given file path.
    /// </summary>
    public static NoteEventDataWrapper GetNoteEventsFromFilePath(string filePath)
    {
        if (File.Exists(filePath))
        {

            if (Path.GetExtension(filePath) == ".json" || Path.GetExtension(filePath) == ".replay")
            {
                return GetDataFile(filePath);
            }
            else if (Path.GetExtension(filePath) == ".mid")
            {
                return ReadMidiFile(filePath);
            }
            else return null;
        }
        else
        {
            Debug.LogError("NO .JSON/.REPLAY/.MID FILE FOUND AT PATH: "+ filePath);
            return null;
        }

    }

    /// <summary>
    /// Counts total number of notes from the currently selected song, filtering out tempo change events.
    /// </summary>
    /// <returns> total number of notes in the currently selected song.</returns>
    public static int CountNotes()
    {


        
        if (File.Exists(GameSettings.currentSongPath))
        {
            if (Path.GetExtension(GameSettings.currentSongPath)==".json")
            {
                int count = 0;
                foreach (var note in GetDataFile(GameSettings.currentSongPath).NoteEvents)
                {
                    if(note.startTime == Mathf.NegativeInfinity||note.noteNumber == int.MinValue)
                    {
                       //do nothin
                    }
                    else { count = 0;  }
                }
                return count;
            }

            else if (Path.GetExtension(GameSettings.currentSongPath) == ".mid")
            {
                return CountFromMidi();
            }
            else
            {
                Debug.LogError("Current Song Path Not Json or Midi");
                return int.MinValue;
            }
        }
        
        else
        {
            Debug.LogError("NO .JSON/.MID FILE FOUND AT PATH: " + GameSettings.currentSongPath);
            return -1;
        }

        int CountFromMidi()
        {
            int noteOnCount = 0;
            // Load the MIDI file
            MidiFile midiFile = new MidiFile(GameSettings.currentSongPath, false);

            // Iterate through all track chunks in the MIDI file
            foreach (var trackChunk in midiFile.Events)
            {
                // Iterate through all MIDI events in the track chunk
                foreach (var midiEvent in trackChunk)
                {
                    // Check if the MIDI event is a NoteOn event
                    if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        noteOnCount++;
                    }
                }
            }

            return noteOnCount;
        }
    }

    /// <summary>
    /// Get <see cref="NoteEventDataWrapper"/> from Json data with given path.
    /// </summary>

    public static NoteEventDataWrapper GetDataFile(string jsonFilePath)
    {
        return MidiDataHandler.GetJSONData(jsonFilePath);
    }
    /// <summary>
    /// Get <see cref="NoteEventDataWrapper"/> from Midi data with given path.
    /// </summary>

    static NoteEventDataWrapper ReadMidiFile(string midiFilePath)
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
                        ProcessTempoChange(tempoEvent);
                    }
                   
                    // Add more conditions as needed for other meta-event types
                }
            }
        }
        if (bpm == 0) { Debug.LogError("BPM WAS NOT FOUND/ IS 0."); return null; }
        return MidiDataHandler.SaveNoteEventData(".json", bpm, noteEvents);


        void ProcessTempoChange(TempoEvent tempoChangeEvent)
        {
            float bpm = 60000000f / tempoChangeEvent.MicrosecondsPerQuarterNote;
            noteEvents.Add(new NoteEventInfo(bpm));
        }

        void ProcessNoteOnEvent(NoteEvent noteOnEvent)//```````````````````````````````````````````````````````modify so if you (somehow ) press a note 2 times before you let go of the first note, it marks its end time.
        {
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


