using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class MidiInput : MonoBehaviour
{
    public static MidiInput instance;
    /// <summary>
    /// Currently loaded notes for the selected song.
    /// </summary>
    public List<NoteEventInfo> storedNoteEvents;
    
    /// <summary>
    /// Reference to the <see cref="GameManager.PrepareNotes(float, List{NoteEventInfo}, bool)"/> prepareNotes Coroutine 
    /// </summary>
    public Coroutine PrepareNotesCoroutine;
    /// <summary>
    /// Is the pedal pressed.
    /// </summary>
    public bool isPedalPressed;
    /// <summary>
    /// array of bools corresponding to currently activated keys.
    /// </summary>
    public bool[] enabledKeys = new bool[88];
    /// <summary>
    /// Should the game take piano input.
    /// </summary>
    public bool takeInput = true;
    /// <summary>
    /// Are we in the game scene.
    /// </summary>
    public bool inGame = false;

    /// <summary>
    /// are midi devices hooked?<br/>
    /// <see cref="HookMidiDevice"/> and <see cref="UnHookMidiDevice"/>.
    /// </summary>
    public bool isMidiHooked;

    /// <summary>
    /// Render texture used for previewing scenes in Song Selection.
    /// </summary>
    public RenderTexture renderTexture;
    /// <summary>
    /// keyPrefab object with a RawImage component to assign <see cref="renderTexture"/> to.
    /// </summary>
    [SerializeField] GameObject imagePrefab;
    
    /// <summary>
    /// Dictionary mapping for Computer keyboard input to note input.
    /// </summary>
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
    };

    /// <summary>
    /// Scene used for previewing songs in Song Selection.
    /// </summary>
    public Scene currentPreview;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopSong();
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
 /// Loads a preview of selected song into the <see cref="currentPreview"/> Scene variable, and begins playing the songs preview.
 /// </summary>
 /// <param name="sceneName"></param>
    public void LoadScenePreview(string sceneName)
    {

        if (currentPreview.isLoaded)
        {
            SceneManager.UnloadSceneAsync(currentPreview);
        }
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOperation.completed += (operation) =>
        {
            currentPreview = SceneManager.GetSceneByName(sceneName); // Assign the loaded scene to currentPreview
            OnSceneLoaded(operation);
        };


        void OnSceneLoaded(AsyncOperation asyncOperation)
        {

            Scene previewScene = SceneManager.GetSceneByName(sceneName);


            // Iterate through the root game objects of the scene
            foreach (GameObject rootObject in previewScene.GetRootGameObjects())
            {
                if (rootObject.TryGetComponent(out TransitionManager transition))
                {
                    transition.GetComponent<Canvas>().enabled = false;
                }
                if (rootObject.TryGetComponent(out Canvas canvas))
                {
                    canvas.enabled = false;
                }
                // Assign the object to the "PreviewLayer"
                AssignToPreviewLayer(rootObject);

                // Check if the object has an EventSystem component and destroy it
                if (rootObject.TryGetComponent(out EventSystem eventSystem))
                {
                    Destroy(eventSystem.gameObject);
                }

                // Find the camera in the preview scene
                Camera camera = rootObject.GetComponentInChildren<Camera>();
                if (camera != null)
                {

                    // Set the camera to render to the specified RenderTexture
                    camera.cullingMask = 1 << LayerMask.NameToLayer("PreviewLayer");
                    camera.targetTexture = renderTexture;
                }
            }

            // Create a new UI Image object
            GameObject imageObject = Instantiate(imagePrefab, UiHolder.instance.transform);
            RawImage image = imageObject.GetComponentInChildren<RawImage>(); // CAN ONLY DO THIS BECAUSE IT IS RAWIMAGE, SINCE I USE MANY NORMAL IMAGE TYPES.


            // Check if RawImage component exists
            if (image != null)
            {
                // Assign the RenderTexture to the RawImage component's texture
                image.texture = renderTexture;
                if (UiHolder.instance.scenePreview != null) { Destroy(UiHolder.instance.scenePreview); }
                UiHolder.instance.scenePreview = imageObject;
            }
            else
            {
                Debug.LogError("RawImage component not found on the instantiated image prefab.");
            }
            asyncOperation.completed -= OnSceneLoaded;

        }

        // Method to assign an object and its children to the "PreviewLayer"
        void AssignToPreviewLayer(GameObject obj)
        {
            // Assign the object to the "PreviewLayer"
            obj.layer = LayerMask.NameToLayer("PreviewLayer");

            // Recursively assign children to the "PreviewLayer"
            foreach (Transform child in obj.transform)
            {
                AssignToPreviewLayer(child.gameObject);
            }
        }
    }



    /// <summary>
    /// Loads the selected song for gameplay.
    /// </summary>

    public void LoadSongFromCurrentSettings(bool isPreview = false)
    {
        MP3Handler.instance.StopMusic();
        string gameMode;
        if (KiboardDebug.isMidiConnected && GameSettings.usePiano) { gameMode = "GameScene88"; } else { gameMode = "GameScene12"; }// determine if player should load into 88 oor 12 key game scene based on if they have a midi device.

        if (isPreview)
        {
            LoadScenePreview(gameMode);
            NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromFilePath(GameSettings.currentSongPath);
            GameSettings.bpm = data.BPM;
            storedNoteEvents = data.NoteEvents;
            takeInput = false;
            inGame = false;
            StartCoroutine(StartSong(true));
        }
        else
        {

            try
            {
                TransitionManager.instance.LoadNewScene(gameMode);
            }

            catch (Exception e)
            {
                Debug.Log(e);
                SceneManager.LoadScene(gameMode);
            }

            if (GameSettings.usePiano) { HookMidiDevice(); } else { UnHookMidiDevice(); }
            NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromFilePath(GameSettings.currentSongPath);
            GameSettings.bpm = GameSettings.bpm == 0 ? data.BPM : GameSettings.bpm;

            storedNoteEvents = data.NoteEvents;
            takeInput = true;
            inGame = true;
            if (PrepareNotesCoroutine != null) StopCoroutine(PrepareNotesCoroutine);
            GameManager.instance.StopReadiedNotes();
            StartCoroutine(StartSong());
        }



    }

    public void LoadSongFromNoteEvents(List<NoteEventInfo> noteEvents, float bpm,bool isPreview = false)
    {
        MP3Handler.instance.StopMusic();
        string gameMode;
        if (KiboardDebug.isMidiConnected && GameSettings.usePiano) { gameMode = "GameScene88"; } else { gameMode = "GameScene12"; }// determine if player should load into 88 oor 12 key game scene based on if they have a midi device.

        if (isPreview)
        {
            LoadScenePreview(gameMode);
            NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromFilePath(GameSettings.currentSongPath);
            GameSettings.bpm = data.BPM;
            storedNoteEvents = data.NoteEvents;
            takeInput = false;
            inGame = false;
            StartCoroutine(StartSong(true));
        }
        else
        {

            try
            {
                TransitionManager.instance.LoadNewScene(gameMode);
            }

            catch (Exception e)
            {
                Debug.Log(e);
                SceneManager.LoadScene(gameMode);
            }

            if (GameSettings.usePiano) { HookMidiDevice(); } else { UnHookMidiDevice(); }
            storedNoteEvents = noteEvents;
            GameSettings.bpm = GameSettings.bpm == 0 ? bpm: GameSettings.bpm;
            takeInput = true;
            inGame = true;
            if (PrepareNotesCoroutine != null) StopCoroutine(PrepareNotesCoroutine);
            GameManager.instance.StopReadiedNotes();
            Replay.recordReplay = false;
            StartCoroutine(StartSong());
        }



    }
    /// <summary>
    /// Hooks midi devices to be used for input.
    /// </summary>
    public void HookMidiDevice()
    {
        try
        {
            if (isMidiHooked) { Debug.Log("Midi Already Hooked"); return; }
            MidiMaster.noteOnDelegate += NoteOn;
            MidiMaster.noteOffDelegate += NoteOff;
            MidiMaster.knobDelegate += PedalStateChanged;
            isMidiHooked = true;
        }
        catch
        {
            isMidiHooked = false;
            Debug.Log("Error Hooking midi device");
        }
    }
    /// <summary>
    /// unhooks midi devices to remove their input.
    /// </summary>
    public void UnHookMidiDevice()
    {
        
        try
        {
            MidiMaster.noteOnDelegate -= NoteOn;
            MidiMaster.noteOffDelegate -= NoteOff;
            MidiMaster.knobDelegate -= PedalStateChanged;
            isMidiHooked = false;
        }
        catch
        {
            isMidiHooked = false;
            Debug.Log("Error Unhooking midi device");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns><see cref="NoteEventDataWrapper"/> loaded from the currently selected song</returns>
    public NoteEventDataWrapper GetNoteEventWrapperFromSelectedSong()
    {
        NoteEventDataWrapper data = MidiReadFile.GetNoteEventsFromFilePath(GameSettings.currentSongPath);
        if (data == null) { Debug.Log("Error Reading data from path: " + GameSettings.currentSongPath); return null; }
        storedNoteEvents = data.NoteEvents;
        return data;
    }
    /// <summary>
    /// called to begin playing the song.
    /// </summary>
    public void StartSongCoroutine()
    {
        StartCoroutine(StartSong());
    }
    /// <summary>
    /// Starts playing the loaded song using the current <see cref="FileGroup"/>'s mp3 file field.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartSong(bool isPreview = false)
    {

        GameManager.instance.currentSongScore?.ClearScore();
        GameManager.instance.combo?.ClearCombo();
        yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotes(GameSettings.bpm, storedNoteEvents, isPreview));
        StartCoroutine(MP3Handler.instance.PlaySong(SongSelection.GetUnderscoreSubstring(GameSettings.currentFileGroup.Mp3File)));
    }
    /// <summary>
    /// Starts playing the loaded song from a given file path.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartSong(string mp3Path, bool isPreview = false)
    {

        GameManager.instance.currentSongScore?.ClearScore();
        GameManager.instance.combo?.ClearCombo();
        yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotes(GameSettings.bpm, storedNoteEvents, isPreview));
        StartCoroutine(MP3Handler.instance.PlaySong(mp3Path));
    }
    /// <summary>
    /// Starts playing a specified list of <see cref="NoteEventInfo"/>
    /// </summary>
    /// <param name="loadEvents">The list of note events to play.</param>
    /// <returns></returns>
    public IEnumerator StartSong(List<NoteEventInfo> loadEvents, bool isPreview = false)
    {
        Debug.Log("start song");
        GameManager.instance.currentSongScore?.ClearScore();
        var bpm = GameSettings.bpm;
        loadEvents.ForEach(noteEvent => noteEvent.noteNumber += 20); // i - for the fucking life of me- cannot figure out why directly processing the midi files makes the note numbers
                                                                     // 20 higher, but i have to do this to match that with the song editor.
        yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotes(GameSettings.bpm, storedNoteEvents, isPreview));
        Debug.Log("past prep notes");

        StartCoroutine(MP3Handler.instance.PlaySong(GameSettings.currentSongPath));

    }

    public IEnumerator StartSong(List<NoteEventInfo> loadEvents,string mp3Path, bool isPreview = false)
    {
        GameManager.instance.currentSongScore?.ClearScore();
        var bpm = 130; //   GameSettings.bpm;
        loadEvents.ForEach(noteEvent => noteEvent.noteNumber += 20); // i - for the fucking life of me- cannot figure out why directly processing the midi files makes the note numbers
                                                                     // 20 higher, but i have to do this to match that with the song editor.
        yield return PrepareNotesCoroutine = StartCoroutine(GameManager.instance.PrepareNotes(bpm, loadEvents, isPreview));

        StartCoroutine(MP3Handler.instance.PlaySong(mp3Path));

    }

    /// <summary>
    /// Stops the currently playing song.
    /// </summary>
    public void StopSong()
    {
        if (GameManager.instance.isCurSongPreview) { return; }
        GameManager.instance.StopSong();
        StopCoroutine(PrepareNotesCoroutine);
        MP3Handler.instance.StopMusic();

        GameSettings.ResetSettings(false);
        GameManager.instance.ReturnToSongSelection();


    }

    /// <summary>
    /// Handles MIDI note on events.
    /// </summary>
    /// <param name="channel">The MIDI channel.</param>
    /// <param name="note">The MIDI note number.</param>
    /// <param name="velocity">The note velocity.</param>
    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        if (!takeInput) { return; }
        // Check if the received note and timing match any stored events
        if (inGame)
        {
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

            score ??= GetTimingScore(timing);
            GameManager.instance.combo.ChangeMultiplier(score);

            SpawnPiano.instance.UpdateKeyColours(note - 21, true, score);
            if (GameManager.instance.songTime >= 0)
            {
                GameManager.instance.UpdatePlayerScore(score);
            }
            Replay.UpdateReplay(note, GameManager.instance.songTime);
        }
        enabledKeys[note - 21] = true;



        
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
        if (GameSettings.usePiano) { return noteNumber == storedNote.noteNumber && !storedNote.triggered && timing < 0.5f; }
        return noteNumber % 12 == storedNote.noteNumber % 12 && !storedNote.triggered && timing < 0.5f;

    }


    /// <summary>
    /// Handles MIDI note off events.
    /// </summary>
    /// <param name="channel">The MIDI channel.</param>
    /// <param name="note">The MIDI note number.</param>
    void NoteOff(MidiChannel channel, int note)
    {
        if (!takeInput || isPedalPressed) { return; }
        if (inGame)
        {
            SpawnPiano.instance.UpdateKeyColours(note - 21, false);
            Replay.UpdateReplay(note, GameManager.instance.songTime);
        }
        enabledKeys[note - 21] = false;

    }
    /// <summary>
    /// Reloads the currently playing song.
    /// </summary>
    public void RetryCurrentSong()
    {
        UnHookMidiDevice();
        LoadSongFromCurrentSettings(false);
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
        //0.125f leniancy
        return Mathf.Abs( (GameManager.instance.songTime + PlayerSettings.inputDelay)  - 0.125f - storedTiming);
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
