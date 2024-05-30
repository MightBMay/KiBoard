





//IF YOU ARE ENCOUNTERING WEIRD ERRORS WITH NOTES ITS PROBABLY DUE TO THE CONSTRUCTOR OVERRIDE HACK LOL

/// <summary>
/// Class containing all info needed to play a note.
/// </summary>
[System.Serializable]
public class NoteEventInfo
{
    /// <summary>
    /// Number of the note ranging from 0-87 coinciding with the notes on a piano.
    /// </summary>
    public int noteNumber;
    /// <summary>
    /// starting time of the note.
    /// </summary>
    public float startTime;
    /// <summary>
    /// release time of the note.
    /// </summary>
    public float endTime;
    /// <summary>
    /// Has this note been triggered already?
    /// </summary>
    public bool triggered;

    public NoteEventInfo()
    {

    }

    public NoteEventInfo(int noteNumber, float startTime, float endTime)
    {
        this.noteNumber = noteNumber;
        this.startTime = startTime;
        this.endTime = endTime;
        this.triggered = false;
    }

    /// <summary>
    /// Override to set bpm this way so that i don't need to make a seperate class and implement it, 
    /// OR store a bpm float with every note. just be sure to check start time when processing them.
    /// </summary>
    /// <param name="Bpm"> new bpm to set to when note is processed.</param>
    public NoteEventInfo(float bpm)
    {
        this.noteNumber = int.MinValue;
        this.startTime = float.NegativeInfinity;
        this.endTime = bpm;
        this.triggered = false;
    }

    /// <summary>
    /// Calculate duration of note.
    /// </summary>
    /// <returns>Float containing note duration.</returns>
    public float GetDuration()
    {
        return endTime - startTime;
    }
    /// <summary>
    /// sets start and end time.
    /// </summary>
    /// <param name="start">new start time.</param>
    /// <param name="end">new end time.</param>
    public void SetStartEndTime(float start, float end)
    {
        startTime = start;
        endTime = end;
    }
}

