using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool inEditor;
    public float songTime;
    public bool startTimer;
    public int beatsToFall=4;

    public int totalNotes;
    float screenHeight;
    float fallSpeed;
    float distanceToFall;
    [SerializeField] float spawnOffset = 2f;
    public GameObject notePrefab;
    float baseNoteScalingFactor = 5.4f; // do not ask me where this number came from.
    public float modifiedNoteScale;
    [SerializeField] Transform noteHolder;
    List<Coroutine> readiedNotes = new();
    string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", };
    public SongScore currentSongScore;
    public SongScore selectedSongHighScore;
    public Combo combo = new();

    static Dictionary<string, int> nameToNoteMap = new()
    {
        {"cb", 1 },
        {"c", 1 },
        {"c#", 2 },
        {"db", 2 },
        {"d", 3 },
        {"d#", 4 },
        {"eb", 4 },
        {"e", 5 },
        {"e#", 6 },
        {"fb", 5 },
        {"f", 6 },
        {"f#", 7 },
        {"gb", 7 },
        {"g", 8 },
        {"g#", 9 },
        {"ab", 9 },
        {"a", 10 },
        {"a#", 11 },
        {"bb", 11 },
        {"b", 12 },
        {"b#", 1 },
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); }
    }
    private void Update()
    {
        if (startTimer)
        {
            songTime += Time.deltaTime;
        }
    }
    /// <summary>
    /// Sets the total number of notes in the song.
    /// </summary>
    /// <param name="noteCount">The total number of notes.</param>
    public void SetSongTotalNotes(int noteCount)
    {
        totalNotes = noteCount;
    }
    /// <summary>
    /// Prepares notes for playback using piano input.
    /// </summary>
    /// <param name="BPM">The BPM (Beats Per Minute) of the song.</param>
    /// <param name="noteEvents">List of note events to prepare.</param>
    public IEnumerator PrepareNotesPiano(float BPM, List<NoteEventInfo> noteEvents) // TEMP 0.5f, change to 5.4f i think`````````````````````````````````````````````````````````````````````````````````````````````````````````
    {
        if (noteEvents == null) { Debug.Log("gameloop noteEvents null"); yield break; }
        songTime = -3;
        modifiedNoteScale = baseNoteScalingFactor * (130 / BPM);
        StopReadiedNotes();
        AssignSongValues();

        yield return new WaitUntil(() => (Input.anyKeyDown || MidiInput.instance.GetAnyNoteActive()));

        noteEvents.ForEach(noteEvent => readiedNotes.Add(StartCoroutine(ReadyNote(noteEvent.startTime - spawnOffset, noteEvent))));

        // Game loop is finished
        yield return null;

        void AssignSongValues()
        {
            spawnOffset = (beatsToFall * 60f / BPM);
            screenHeight = 2f * Camera.main.orthographicSize;
            distanceToFall = screenHeight;
            // Calculate the speed based on the distance and duration
            fallSpeed = (distanceToFall / spawnOffset);
            SetSongTotalNotes(noteEvents.Count);
            SongScore songScore = new();
        }

        IEnumerator ReadyNote(float spawnTime, NoteEventInfo noteEvent)
        {
            yield return new WaitUntil(() => songTime >= spawnTime);
            SpawnNote(noteEvent, spawnTime);
        }


        void SpawnNote(NoteEventInfo noteEvent, float spawnTime)
        {
            // scale/length of the note deterimned by the note duration, and a scaling factor (~~~~~~~~~~~~~~~~~BASE THIS ON MF BPM)``````````````````````````````````````````````````````````````````````````````````
            float noteScale = (noteEvent.endTime - noteEvent.startTime) * modifiedNoteScale;
            //spawn a note and store a reference.
            GameObject noteInstance = Instantiate(notePrefab, new Vector3(-13.2f + (0.20505f * (noteEvent.noteNumber)), (screenHeight) + (noteScale / 2) - 2.5f, 0f), Quaternion.identity);
            FallingNote fallingNote = noteInstance.GetComponent<FallingNote>();
            SpriteRenderer spriteRenderer = noteInstance.GetComponent<SpriteRenderer>();
            fallingNote.velocity = fallSpeed; // set falling speed of the note to the value calculated in AssignSongValues()
            fallingNote.maxYBound = spriteRenderer.bounds.max.y; //Used to determine when a note is far enough off screen to be destroyed.
            fallingNote.GetComponentInChildren<NoteShadow>().SetShadowSize(noteScale + 0.075f);
            if (noteHolder != null)
            {
                noteInstance.transform.SetParent(noteHolder.transform, false);
            }
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, noteScale);


            if (noteEvent == noteEvents[noteEvents.Count - 1]) // check if note is the final note.
            {
                fallingNote.isLast = true;// set flag to end song after the last note is destroyed.

            }
        }
    }
    public void ModifyNoteScale(float bpm)
    {
        modifiedNoteScale = baseNoteScalingFactor * (130 / bpm);
    }

    /// <summary>
    /// Prepares notes for playback using keyboard input.
    /// </summary>
    /// <param name="BPM">The BPM (Beats Per Minute) of the song.</param>
    /// <param name="noteEvents">List of note events to prepare.</param>
    public IEnumerator PrepareNotesKeyboard12(float BPM, List<NoteEventInfo> noteEvents)
    {
        if (noteEvents == null) { Debug.Log("gameloop noteEvents null"); yield break; }
        
        songTime = -3;
        modifiedNoteScale = baseNoteScalingFactor * (130 / BPM);
        StopReadiedNotes();
        AssignSongValues();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~REPLACE THIS WITH WHATEVER INPUT IS TO GO BACK~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        yield return new WaitUntil(() => ((Input.anyKeyDown) || MidiInput.instance.GetAnyNoteActive())); // wait for any input to start.
        noteEvents.ForEach(noteEvent => readiedNotes.Add(StartCoroutine(ReadyNote(noteEvent.startTime - spawnOffset, noteEvent))));
        // Game loop is finished
        yield return null;

        void AssignSongValues()
        {
            spawnOffset = (beatsToFall * 60f / BPM); // notes will spawn beatstofall #beats before reaching the hit spot.
            screenHeight = 2f * Camera.main.orthographicSize; // base fall distance off camera height.
            distanceToFall = screenHeight;
            // Calculate the speed based on the distance and duration
            fallSpeed = (distanceToFall / spawnOffset);
            SetSongTotalNotes(noteEvents.Count); // sets the total note count.
            SongScore songScore = new(); // resets scoring for the song.
        }

        IEnumerator ReadyNote(float spawnTime, NoteEventInfo noteEvent)
        { // waits until spawntime, then spawns note.
            yield return new WaitUntil(() => songTime >= spawnTime);
            SpawnNote(noteEvent, spawnTime);
        }

        void SpawnNote(NoteEventInfo noteEvent, float spawnTime)
        {
            // scale/length of the note deterimned by the note duration, and a scaling factor (~~~~~~~~~~~~~~~~~BASE THIS ON MF BPM)``````````````````````````````````````````````````````````````````````````````````
            float noteScale = (noteEvent.endTime - noteEvent.startTime) * modifiedNoteScale;
            //spawn a note and store a reference.
            GameObject noteInstance = Instantiate(notePrefab, new Vector3(-13.2f + (0.20505f * (48 + (noteEvent.noteNumber % 12))), ( screenHeight )+ (noteScale / 2) - 2.5f, 0f), Quaternion.identity);
            FallingNote fallingNote = noteInstance.GetComponent<FallingNote>();
            SpriteRenderer spriteRenderer = noteInstance.GetComponent<SpriteRenderer>();
            fallingNote.velocity = fallSpeed; // set falling speed of the note to the value calculated in AssignSongValues()
            fallingNote.maxYBound = spriteRenderer.bounds.max.y; //Used to determine when a note is far enough off screen to be destroyed.
            fallingNote.GetComponentInChildren<NoteShadow>().SetShadowSize(noteScale + 0.075f);

            spriteRenderer.size = new Vector2(spriteRenderer.size.x, noteScale);


            if (noteEvent == noteEvents[noteEvents.Count - 1]) // check if note is the final note.
            {
                fallingNote.isLast = true;// set flag to end song after the last note is destroyed.

            }
        }
    }
    /// <summary>
    /// Stops all coroutines for preparing notes.
    /// </summary>
    public void StopReadiedNotes()
    {
        int count = 0;
        foreach (Coroutine c in readiedNotes)
        {
            StopCoroutine(c);
            count++;
        }
        readiedNotes.Clear();
    }
    /// <summary>
    /// Updates the player's score based on the hit timing.
    /// </summary>
    /// <param name="score">The hit score (Perfect, Good, Okay, Miss).</param>

    public void UpdatePlayerScore(string score)
    {
        currentSongScore ??= new();
        currentSongScore.AddScore(score, combo.multiplier);

    }
    /// <summary>
    /// Converts a MIDI note number to its corresponding name.
    /// </summary>
    /// <param name="noteNumber">The MIDI note number.</param>
    /// <returns>The note name (e.g., C, C#, D).</returns>
    public string ConvertNoteNumberToName(int noteNumber)
    {

        int octave = (noteNumber / 12) - 1;

        int noteIndex = (noteNumber) % 12;
        string noteName = noteNames[noteIndex];
        if (GameSettings.usePiano)
        {
            return $"{noteName} {octave}";
        }
        return $"{noteName}";
    }
    /// <summary>
    /// Converts a note name to its corresponding MIDI note number.
    /// </summary>
    /// <param name="noteName">The note name (e.g., C, C#, D).</param>
    /// <returns>The MIDI note number.</returns>
    public static int ConvertNoteNameToNumber(string noteName)
    {
        if (!char.IsDigit(noteName[0]))
        {
            int octave = 0;

            for (int i = 0; i < noteName.Length; i++)
            {
                if (char.IsDigit(noteName[i]))
                {
                    octave = noteName[i];
                }
            }
            nameToNoteMap.TryGetValue(noteName.ToLower(), out int value);

            return value + (12 * octave);
        }
        return int.Parse(noteName);

    }

    /// <summary>
    /// Coroutine for handling song end actions.
    /// </summary>
    public IEnumerator OnSongEnd()
    {
        startTimer = false;
        FinalizeScore( currentSongScore.GetScoreArray(totalNotes));
       
        yield return new WaitForSeconds(5f);
        ReturnToSongSelection();
        GameSettings.ResetSettings(false);
        //`````````````````````````````````````````````````````````````````````````````````````````````````` make this open some sort of ui with retry, back to song selection scene, etc.
    }
    /// <summary>
    /// Returns to the song selection scene.
    /// </summary>
    public void ReturnToSongSelection()
    {
        startTimer = false;
        inEditor = false;
        StopReadiedNotes();
        MidiInput.instance.inGame = false;

        try { TransitionManager.instance.LoadNewScene("SongSelect"); }
        catch { SceneManager.LoadScene("SongSelect"); }

    }


    public void StopSong()
    {
        startTimer = false;
        StopReadiedNotes();
        MidiInput.instance.inGame = false;
    }

    /// <summary>
    /// Enters the song editor mode.
    /// </summary>
    public void EnterSongEditor()
    {
        inEditor = true;
        string songName = GameSettings.currentSongName;
        if (string.IsNullOrEmpty(songName)) { return; }
        try { TransitionManager.instance.LoadNewScene("SongEditorScene"); }
        catch { SceneManager.LoadScene("SongEditorScene"); }
        MidiInput.instance.GetBPM(songName);
    }
    /// <summary>
    /// Modifies the note scale based on the BPM (Beats Per Minute).
    /// </summary>
    /// <param name="BPM">The BPM of the song.</param>
    /// <returns>The modified note scale.</returns>


    public void FinalizeScore(int[] score)
    {
        Debug.Log($"Total Score: {score[0]} |     Perfect: {score[1]}, Good: {score[2]}, Okay: {score[3]}, Extra: {score[4]}, Missed: {score[5]}");
        Debug.Log("Longest Combo: " + combo.highestCount);
        currentSongScore.FinalizeScore();
    }

    public void RefreshJsonFiles()
    {
        NoteEventDataWrapper temp = MidiReadFile.GetNoteEventsFromMidiFileName(GameSettings.currentSongName);
        MidiDataHandler.SaveNoteEventData(GameSettings.currentSongName, temp.BPM, temp.NoteEvents);

    }


}


