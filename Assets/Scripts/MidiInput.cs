using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MidiInput : MonoBehaviour
{
    public static MidiInput instance;
    public List<NoteEventInfo> storedNoteEvents;
    Coroutine PrepareNotesCoroutine;


    public bool isPedalPressed;
    public bool[] enabledKeys = new bool[88];
    public bool inGame = false;
    public float inputDelay;

    // Define the mapping between keyboard keys and MIDI note numbers
    Dictionary<KeyCode, int> keyboard12 = new Dictionary<KeyCode, int>
    {
        { KeyCode.A, 48 },
        { KeyCode.W, 49 },
        { KeyCode.S, 50 },
        { KeyCode.E, 51 },
        { KeyCode.D, 52 },
        { KeyCode.F, 53 },
        { KeyCode.T, 54 },
        { KeyCode.G, 55 },
        { KeyCode.Y, 56 },
        { KeyCode.H, 57 },
        { KeyCode.U, 58 },
        { KeyCode.J, 59 },
        { KeyCode.K, 60 },
    };
    Dictionary<KeyCode, int> keyboard8 = new Dictionary<KeyCode, int>
    {
        { KeyCode.A, 48 },
        { KeyCode.S, 50 },
        { KeyCode.D, 52 },
        { KeyCode.F, 53 },
        { KeyCode.G, 55 },
        { KeyCode.H, 57 },
        { KeyCode.J, 59 },
        { KeyCode.K, 60},
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
        MidiMaster.knobDelegate += PedalStateChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopSong(GameManager.instance.inEditor);
        }
        CheckNotesKeyboard12();
        //CheckNotesKeyboard8();

    }
    /// <summary>
    /// Handles MIDI sustain pedal state changes.
    /// </summary>
    /// <param name="channel">The MIDI channel.</param>
    /// <param name="knobNumber">The knob number.</param>
    /// <param name="knobValue">The knob value.</param>
    void PedalStateChanged(MidiChannel channel, int knobNumber, float knobValue)
    {
        if (knobNumber == 64 && knobValue > 0.5f)
        {
            isPedalPressed = true;

        }
        else
        {
            isPedalPressed = false;
            try { SpawnPiano.instance.ClearAllKeyColours(); }
            catch { }
        }
    }
    /// <summary>
    /// Loads the selected song for gameplay.
    /// </summary>
    public void LoadSongFromCurrentSettings()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        var currentSettings = SettingsManager.instance.gameSettings;
        NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromName(currentSettings.currentSongName);
        currentSettings.bpm = currentSettings.bpm == 0 ? data.BPM : currentSettings.bpm;
        GameManager.instance.modifiedNoteScale = GameManager.instance.baseNoteScalingFactor * (130 / data.BPM);

        storedNoteEvents = data.NoteEvents;
        inGame = true;
        StartCoroutine(StartSong());

    }

    public NoteEventDataWrapper GetNoteEventWrapperFromSelectedSong(GameSettings currentSettings)
    {
        NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromName(currentSettings.currentSongName);
        storedNoteEvents = data.NoteEvents;
        return data;
    }
    /// <summary>
    /// Starts playing the loaded song.
    /// </summary>
    /// <returns>Coroutine for preparing notes and playing the song.</returns>
    public IEnumerator StartSong()
    {
        GameManager.instance.currentSongScore.ClearScore();
        var currentSettings = SettingsManager.instance.gameSettings;
        if (currentSettings.usePiano)
        {
            GameManager.instance.modifiedNoteScale = GameManager.instance.baseNoteScalingFactor * (130 / SettingsManager.instance.gameSettings.bpm);
            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesPiano(currentSettings.bpm, storedNoteEvents));
        }
        else
        {
            GameManager.instance.modifiedNoteScale = GameManager.instance.baseNoteScalingFactor * (130 / SettingsManager.instance.gameSettings.bpm);
            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesKeyboard12(currentSettings.bpm, storedNoteEvents));
        }

        StartCoroutine(MP3Handler.instance.PlaySong(currentSettings.currentSongName));
        GameManager.instance.startTimer = true;
    }
    /// <summary>
    /// Starts playing a song with specified note events.
    /// </summary>
    /// <param name="loadEvents">The list of note events to play.</param>
    /// <returns>Coroutine for preparing notes and playing the song.</returns>
    public IEnumerator StartSong(List<NoteEventInfo> loadEvents)
    {
        GameManager.instance.currentSongScore.ClearScore();
        var bpm = SettingsManager.instance.gameSettings.bpm;
        loadEvents.ForEach(noteEvent => noteEvent.noteNumber += 20); // i - for the fucking life of me- cannot figure out why directly processing the midi files makes the note numbers
                                                                   // 20 higher, but i have to do this to match that with the song editor.

        if (SettingsManager.instance.gameSettings.usePiano)
        {

            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesPiano(bpm, loadEvents));
        }
        else
        {
            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesKeyboard12(bpm, loadEvents));
        }
        StartCoroutine(MP3Handler.instance.PlaySong(SettingsManager.instance.gameSettings.currentSongName));
        GameManager.instance.startTimer = true;

    }

    /// <summary>
    /// Stops the currently playing song.
    /// </summary>
    public void StopSong(bool inEditor)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.StopSong();
            StopCoroutine(PrepareNotesCoroutine);
            MP3Handler.instance.StopMusic();
            if (inEditor)
            {
                FindObjectOfType<SongEditor>().noteHolder.gameObject.SetActive(true);
                FindObjectOfType<SongNoteEditor>().enabled = true;
                
                foreach(FallingNote note in FindObjectsOfType<FallingNote>())
                {
                    Destroy(note.gameObject);
                }
                return;
            }
            SettingsManager.instance.gameSettings.ResetSettings();
            GameManager.instance.ReturnToSongSelection();

        }
    }

    /// <summary>
    /// Handles MIDI note on events.
    /// </summary>
    /// <param name="channel">The MIDI channel.</param>
    /// <param name="note">The MIDI note number.</param>
    /// <param name="velocity">The note velocity.</param>
    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        if (!inGame) { return; }
        // Check if the received note and timing match any stored events
        string score = "";
        float timing = Mathf.Infinity;
        foreach (NoteEventInfo storedNote in storedNoteEvents)
        {
            if (storedNoteEvents == null) { return; }
            timing = GetTimeDifference(storedNote.startTime);
            if (IsNoteCorrect(note, timing, storedNote))
            {
                OnNoteSuccess(ref score, note, timing, storedNote);
                break;  // Exit the loop after the first match

            }

        }
        enabledKeys[note - 21] = true;
        score ??= GetTimingScore(timing);
        SpawnPiano.instance.UpdateKeyColours(note - 21, true, score);
        GameManager.instance.UpdatePlayerScore(score);


    }

    /// <summary>
    /// Handles successful note presses
    /// </summary>
    /// <param name="score"></param>
    /// <param name="note"></param>
    /// <param name="timing"></param>
    /// <param name="storedNote"></param>
    public void OnNoteSuccess(ref string score, int note, float timing, NoteEventInfo storedNote)
    {
        score = GetTimingScore(timing);
        SpawnPiano.instance.SpawnKeyParticle(note - 21, score);
        storedNote.triggered = true;
    }
    /// <summary>
    /// Check if note is was played at a correct timing.
    /// </summary>
    /// <param name="noteNumber">Number of the note played.</param>
    /// <param name="timing"> What time the note was hit.</param>
    /// <param name="storedNote">Note that is being compared to.</param>
    /// <returns></returns>
    public bool IsNoteCorrect(int noteNumber, float timing, NoteEventInfo storedNote)
    {
        if (SettingsManager.instance.gameSettings.usePiano) return noteNumber == storedNote.noteNumber && !storedNote.triggered && timing < 0.5f;
        return noteNumber % 12 == storedNote.noteNumber % 12 && !storedNote.triggered && timing < 0.5f;

    }


    /// <summary>
    /// Handles MIDI note off events.
    /// </summary>
    /// <param name="channel">The MIDI channel.</param>
    /// <param name="note">The MIDI note number.</param>
    void NoteOff(MidiChannel channel, int note)
    {
        if (!inGame || isPedalPressed) { return; }
        enabledKeys[note - 21] = false;
        SpawnPiano.instance.UpdateKeyColours(note - 21, false);
    }

    /// <summary>
    /// Checks for pressed keys in the 12-key keyboard layout.
    /// </summary>
    void CheckNotesKeyboard12()
    {
        // Check for computer keyboard key presses and releases
        foreach (var keyValuePair in keyboard12)
        {
            KeyCode key = keyValuePair.Key;
            int noteNumber = keyValuePair.Value;

            if (Input.GetKeyDown(key))
            {
                // Simulate Note On event for the pressed key
                NoteOn(MidiChannel.All, noteNumber, 1.0f);
            }

            if (Input.GetKeyUp(key))
            {
                // Simulate Note Off event for the released key
                NoteOff(MidiChannel.All, noteNumber);
            }
        }
    }
    /// <summary>
    /// Checks for pressed keys in the 8-key keyboard layout.
    /// </summary>
    void CheckNotesKeyboard8()
    {
        // Check for computer keyboard key presses and releases
        foreach (var keyValuePair in keyboard12)
        {
            KeyCode key = keyValuePair.Key;
            int noteNumber = keyValuePair.Value;

            if (Input.GetKeyDown(key))
            {
                // Simulate Note On event for the pressed key
                NoteOn(MidiChannel.All, noteNumber, 1.0f);
            }

            if (Input.GetKeyUp(key))
            {
                // Simulate Note Off event for the released key
                NoteOff(MidiChannel.All, noteNumber);
            }
        }
    }

    /// <summary>
    /// Checks if any note is currently active (pressed).
    /// </summary>
    /// <returns>True if any note is active, otherwise false.</returns>
    public bool GetAnyNoteActive()
    {
        foreach (bool b in enabledKeys)
        {
            if (b)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates the time difference between the stored timing and the current song time.
    /// </summary>
    /// <param name="storedTiming">The stored timing value.</param>
    /// <returns>The time difference.</returns>
    float GetTimeDifference(float storedTiming)
    {
        // Adjust this threshold based on your timing accuracy requirements
        // float timingThreshold = currentSettings.timeInterval;

        // Check if the received timing is within a certain threshold of the stored timing

        return Mathf.Abs(GameManager.instance.songTime - inputDelay - storedTiming);
    }
    /// <summary>
    /// Gets the timing score based on the timing difference.
    /// </summary>
    /// <param name="timing">The timing difference.</param>
    /// <returns>The timing score (e.g., "Perfect", "Good", "Okay", "Miss").</returns>
    public string GetTimingScore(float timing)
    {
        if (timing < 0.1f) { return "Perfect"; }
        else if (timing < 0.175f) { return "Good"; }
        else if (timing < 0.275f) { return "Okay"; }
        else { return "Miss"; }
    }
}
