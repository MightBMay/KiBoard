using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool inEditor;
    public float songTime;
    public bool startTimer;
    public int beatsBeforeNote = 4;

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
    [SerializeField] string scoreString;

    static Dictionary<string, int> nameToNoteMap = new()
    {
        { "cb", 1 },
        { "c", 1 },
        { "c#", 2 },
        { "db", 2 },
        { "d", 3 },
        { "d#", 4 },
        { "eb", 4 },
        { "e", 5 },
        { "e#", 6 },
        { "fb", 5 },
        { "f", 6 },
        { "f#", 7 },
        { "gb", 7 },
        { "g", 8 },
        { "g#", 9 },
        { "ab", 9 },
        { "a", 10 },
        { "a#", 11 },
        { "bb", 11 },
        { "b", 12 },
        { "b#", 1 },
    };

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
        }
        else { Destroy(gameObject); }
    }

    public void AssignFpsText(TMP_InputField text)
    {
        text.text = Application.targetFrameRate.ToString();
    }
    public void SetFPS(string str)
    {
        if (int.TryParse(str, out int newFps))
        {
            Application.targetFrameRate = newFps;
        }
        else { Debug.LogError($"FPS Cap of : {str} is invalid"); }
    }
    public void SetInputDelay(string delayStr)
    {
        if (int.TryParse(delayStr, out int delay))
        {
            PlayerSettings.inputDelay = delay;
        }
        else { Debug.Log("Error Parsing input delay"); }
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
    /// Used to check if a noteevent is storing Tempo Data or Note Data
    /// Because i assign tempo changes by setting the noteevent.starttime to negative infinity and the note num to min value, with the tempo as the end time.
    /// 
    /// </summary>
    /// <param name="note"></param>
    /// <returns>True = note data, false = tempo data.</returns>
    public static bool CheckSpawnNote(NoteEventInfo note)
    {
        return !(note.startTime == float.NegativeInfinity && note.noteNumber == int.MinValue);
    }

    public IEnumerator PrepareNotes(float BPM, List<NoteEventInfo> noteEvents, bool isPreview) // TEMP 0.5f, change to 5.4f i think`````````````````````````````````````````````````````````````````````````````````````````````````````````
    {
        if (noteEvents == null) { Debug.Log("gameloop noteEvents null"); yield break; }
        GameType? gameType = GameSettings.gameType = GameSettings.usePiano ? GameType.Key88 : GameType.Key12;

        SetSongTotalNotes(noteEvents.Count);
        SongScore songScore = new();
        if (isPreview) { isCurSongPreview = true; } else { isCurSongPreview = false; }
        songTime = -3f - (130 / BPM);
        if (!isCurSongPreview) { Replay.recordReplay = true; Replay.StartReplayCapture(); }
        else { Replay.recordReplay = false; }
        screenHeight = 40.16f;//2f * Camera.main.orthographicSize;
        AssignSongValues(BPM);
        
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => (Input.anyKeyDown || MidiInput.instance.GetAnyNoteActive()) || isPreview);
        startTimer = true;
        StopReadiedNotes();
        if (gameType == GameType.Key88) { noteEvents.ForEach(noteEvent => readiedNotes.Add(StartCoroutine(ReadyNote88(noteEvent.startTime, noteEvent))));  }
        else if(gameType == GameType.Key12) { noteEvents.ForEach(noteEvent => readiedNotes.Add(StartCoroutine(ReadyNote12(noteEvent.startTime , noteEvent)))); }
        else
        {
            Debug.LogError("Game type not 88 or 12 key mode.");
        }

        // Game loop is finished
        yield return null;

        void AssignSongValues(float BPM)
        {

            spawnOffset = (beatsBeforeNote * 60f / BPM);
            // Calculate the speed based on the distance and duration
            fallSpeed = (screenHeight / spawnOffset);

            modifiedNoteScale = baseNoteScalingFactor * (130 / BPM);
        }

        float TempoChange(NoteEventInfo note)
        {
            BPM = note.endTime;
            float so = (beatsBeforeNote * 60f / BPM); 
            fallSpeed = (screenHeight / so);
            modifiedNoteScale = baseNoteScalingFactor * (130 / BPM);
            return so;
        }
        IEnumerator ReadyNote88(float spawnTime, NoteEventInfo noteEvent)
        {
            
            if (CheckSpawnNote(noteEvent))
            {
                spawnOffset = TempoChange(noteEvent);
                yield break;
                
            }
            float trueSpawnTime = spawnTime - spawnOffset;
            yield return new WaitUntil(() => songTime >= trueSpawnTime);
            SpawnNote88(noteEvent);
        }
        IEnumerator ReadyNote12(float spawnTime, NoteEventInfo noteEvent)
        {
            if (CheckSpawnNote(noteEvent))
            {
                spawnOffset = TempoChange(noteEvent);
                AssignSongValues(noteEvent.endTime);

            }
            float trueSpawnTime = spawnTime - spawnOffset;
            yield return new WaitUntil(() => songTime >= trueSpawnTime);
            SpawnNote12(noteEvent);
        }

        void SpawnNote88(NoteEventInfo noteEvent)
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
            if (isPreview) AssignToPreviewLayer(noteInstance);
        }
        void SpawnNote12(NoteEventInfo noteEvent)
        {
            // scale/length of the note deterimned by the note duration, and a scaling factor  (~~~~~~~~~~~~~~~~~BASE THIS ON MF BPM)``````````````````````````````````````````````````````````````````````````````````
            float noteScale = (noteEvent.endTime - noteEvent.startTime) * modifiedNoteScale;
            //spawn a note and store a reference.
            GameObject noteInstance = Instantiate(notePrefab, new Vector3(-5.6f + (1 * (noteEvent.noteNumber % 12)), (screenHeight) + (noteScale / 2) - 2.5f, 0f), Quaternion.identity);
            FallingNote fallingNote = noteInstance.GetComponent<FallingNote>();
            SpriteRenderer spriteRenderer = noteInstance.GetComponent<SpriteRenderer>();
            fallingNote.velocity = fallSpeed; // set falling speed of the note to the value calculated in AssignSongValues()
            fallingNote.maxYBound = spriteRenderer.bounds.max.y; //Used to determine when a note is far enough off screen to be destroyed.
            fallingNote.GetComponentInChildren<NoteShadow>().SetShadowSize(noteScale + 0.075f);

            spriteRenderer.size = new Vector2(4.5f, noteScale);


            if (noteEvent == noteEvents[noteEvents.Count - 1]) // check if note is the final note.
            {
                fallingNote.isLast = true;// set flag to end song after the last note is destroyed.


            }
            if (isPreview) AssignToPreviewLayer(noteInstance);
        }
    }

    public bool isCurSongPreview;
    void AssignToPreviewLayer(GameObject obj)
    {

        // Assign the object to the "PreviewLayer"
        LayerMask layer = LayerMask.NameToLayer("PreviewLayer");
        try { SceneManager.MoveGameObjectToScene(obj, MidiInput.instance.currentPreview); } catch { }

        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = layer;
        }
    }
    /// <summary>
    /// Stops all coroutines for preparing notes.
    /// </summary>
    public void StopReadiedNotes()
    {
        foreach (Coroutine c in readiedNotes)
        {
            if (c != null)
            {
                StopCoroutine(c);
            }
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
    public void OnSongEnd()
    {
        startTimer = false;
        MidiInput.instance.takeInput = false;
        int[] score = currentSongScore.GetScoreArray(totalNotes);

        if (!isCurSongPreview) { EndSongMessage.instance?.ShowScore($"Total Score: {score[0]}\nPerfect: {score[1]}\nGood: {score[2]}\nOkay: {score[3]}\nExtra: {score[4]}\nMissed: {score[5]}\nLongest Combo: {combo.highestCount}", currentSongScore.FinalizeScore()); }
        else{ FindObjectOfType<EndPreview>()?.EndPreviewFade(); }

        if (!Replay.isPlayingReplay) { MidiDataHandler.SaveNoteEventData(".replay", Replay.instance.replayNoteData); } // only record replays if you arent playing back a replay.

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
        GameSettings.ResetSettings(false);
        MidiInput.instance.UnHookMidiDevice();

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
    /*public void EnterSongEditor()
    {
        inEditor = true;
        string songName = GameSettings.currentSongName;
        if (string.IsNullOrEmpty(songName)) { return; }
        try { TransitionManager.instance.LoadNewScene("SongEditorScene"); }
        catch { SceneManager.LoadScene("SongEditorScene"); }
        MidiInput.instance.GetBPM(songName);
    }*/
    /// <summary>
    /// Modifies the note scale based on the BPM (Beats Per Minute).
    /// </summary>
    /// <param name="BPM">The BPM of the song.</param>
    /// <returns>The modified note scale.</returns>

    public void RefreshJsonFiles()
    {
        NoteEventDataWrapper temp = MidiReadFile.GetNoteEventsFromFilePath(GameSettings.currentSongPath);
        MidiDataHandler.SaveNoteEventData(".json", temp.BPM, temp.NoteEvents);

    }


    public void SetBeatsBeforeDrop(string num)
    {
        if (!int.TryParse(num, out int newNum))
        {
            if (newNum <= 0) { Debug.LogWarning("BeatBeforeDrop Setting attempted to be set to <=0"); return; }
            Debug.LogError("Non Int Input into Beats Before Drop Setting"); return;
        }
        beatsBeforeNote = newNum;
        Debug.Log($"{newNum} Beats before note");
    }



}



