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
        StopSong();
        CheckNotesKeyboard12();
        //CheckNotesKeyboard8();

    }

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
    public void LoadSongFromCurrentSettings()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        var currentSettings = SettingsManager.instance.gameSettings;
        NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromName(currentSettings.currentSongName);
        currentSettings.bpm = currentSettings.bpm == 0 ? data.BPM : currentSettings.bpm;
        storedNoteEvents = data.NoteEvents;
        inGame = true;
        StartCoroutine(StartSong());

    }
    public IEnumerator StartSong()
    {
        GameManager.instance.currentSongScore.ClearScore();
        var currentSettings = SettingsManager.instance.gameSettings;
        if (currentSettings.usePiano)
        {

            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesPiano(currentSettings.bpm, storedNoteEvents));
        }
        else
        {
            yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotesKeyboard12(currentSettings.bpm, storedNoteEvents));
        }

        StartCoroutine(MP3Handler.instance.PlaySong(currentSettings.currentSongName));
        GameManager.instance.startTimer = true;
    }

    public IEnumerator StartSong(List<NoteEventInfo> loadEvents)
    {
        GameManager.instance.currentSongScore.ClearScore();
        var bpm = SettingsManager.instance.gameSettings.bpm;
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
    public void StopSong()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            GameManager.instance.ReturnToSongSelection();
            StopCoroutine(PrepareNotesCoroutine);
            MP3Handler.instance.StopMusic();
            SettingsManager.instance.gameSettings.ResetSettings();
            SceneManager.LoadScene("SongSelect");
        }
    }

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
        score = score ?? GetTimingScore(timing);
        SpawnPiano.instance.UpdateKeyColours(note - 21, true, score);
        GameManager.instance.UpdatePlayerScore(score);


    }

    public void OnNoteSuccess(ref string score, int note, float timing, NoteEventInfo storedNote)
    {
        score = GetTimingScore(timing);
        SpawnPiano.instance.SpawnKeyParticle(note - 21, score);
        storedNote.triggered = true;
    }

    public bool IsNoteCorrect(int noteNumber, float timing, NoteEventInfo storedNote)
    {
        if (SettingsManager.instance.gameSettings.usePiano) return noteNumber == storedNote.noteNumber && !storedNote.triggered && timing < 0.5f;
        return noteNumber % 12 == storedNote.noteNumber % 12 && !storedNote.triggered && timing < 0.5f;

    }

    void NoteOff(MidiChannel channel, int note)
    {
        if (!inGame || isPedalPressed) { return; }
        enabledKeys[note - 21] = false;
        SpawnPiano.instance.UpdateKeyColours(note - 21, false);
    }


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

    float GetTimeDifference(float storedTiming)
    {
        // Adjust this threshold based on your timing accuracy requirements
        // float timingThreshold = currentSettings.timeInterval;

        // Check if the received timing is within a certain threshold of the stored timing

        return Mathf.Abs(GameManager.instance.songTime - inputDelay - storedTiming);
    }

    public string GetTimingScore(float timing)
    {
        if (timing < 0.075f) { return "Perfect"; }
        else if (timing < 0.15f) { return "Good"; }
        else if (timing < 0.275f) { return "Okay"; }
        else { return "Miss"; }
    }
}
