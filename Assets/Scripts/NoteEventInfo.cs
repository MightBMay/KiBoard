[System.Serializable]
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
