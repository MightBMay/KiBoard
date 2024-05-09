[System.Serializable]





//IF YOU ARE ENCOUNTERING WEIRD ERRORS WITH NOTES ITS PROBABLY DUE TO THE CONSTRUCTOR OVERRIDE HACK LOL


public class NoteEventInfo
{
    public int noteNumber;
    public float startTime;
    public float endTime;
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


    public float GetDuration()
    {
        return endTime - startTime;
    }
    public void SetStartEndTime(float start, float end)
    {
        startTime = start;
        endTime = end;
    }
}

