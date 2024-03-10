using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// Manages the song editor functionality.
/// </summary>
public class SongEditor : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the SongEditor class.
    /// </summary>
    public static SongEditor instance;

    /// <summary>
    /// Prefab for the editor note object.
    /// </summary>
    public GameObject editorNotePrefab;

    /// <summary>
    /// List of note objects in the editor.
    /// </summary>
    public List<GameObject> noteObjects;

    /// <summary>
    /// List of note events associated with the note objects.
    /// </summary>
    public List<NoteEventInfo> noteEvents;

    /// <summary>
    /// Transform to hold instantiated notes.
    /// </summary>
    public Transform noteHolder;

    /// <summary>
    /// Transform representing the origin of the keys.
    /// </summary>
    public Transform keyOrigin;

    public bool isTesting;
    public Coroutine SongTestCoroutine;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        LoadSongAsEditorNotes(SettingsManager.instance.gameSettings);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        // Debugging functionality
        DebugStuff();
    }

    /// <summary>
    /// Debugging method to perform certain actions based on input.
    /// </summary>
    private void DebugStuff()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Start testing the song
           SongTestCoroutine=  StartCoroutine(StartSongTest());
        }
    }




    void LoadSongAsEditorNotes(GameSettings settings)
    {
        NoteEventDataWrapper notes = MidiInput.instance.GetNoteEventWrapperFromSelectedSong(settings);
        settings.bpm = notes.BPM;
        foreach (var note in notes.NoteEvents)
        {
            note.noteNumber -= 20;
        }
        foreach (var note in notes.NoteEvents)
        {
            CreateNote(note.noteNumber, note.startTime, note.endTime, (note.startTime+note.endTime)/2 );
        }
    }


    /// <summary>
    /// Starts testing the song by invoking a coroutine to play the song.
    /// </summary>
    public IEnumerator StartSongTest()
    {
        noteHolder.gameObject.SetActive(false);
        FindObjectOfType<SongNoteEditor>().enabled = false;
        StartCoroutine(MidiInput.instance.StartSong(noteEvents));
        var cameraScroll = FindObjectOfType<CameraScrollManager>();
        cameraScroll.ResetCamera(); 
        yield return new WaitUntil(() => IsTestDone() );
        cameraScroll.canScroll = true;


        bool IsTestDone()
        {
            return !isTesting;
        }
    }

    /// <summary>
    /// Creates a new note object with the specified parameters.
    /// </summary>
    /// <param name="noteNumber">The MIDI note number.</param>
    /// <param name="startTime">The start time of the note event.</param>
    /// <param name="endTime">The end time of the note event.</param>
    /// <param name="mouseHeight">The height of the mouse.</param>
    /// <returns>The created note event.</returns>
    public NoteEventInfo CreateNote(int noteNumber, float startTime, float endTime, float mouseHeight)
    {
        GameObject note = Instantiate(editorNotePrefab, noteHolder);
        noteObjects.Add(note);
        var noteEvent = new NoteEventInfo(noteNumber, startTime, endTime);
        var editorNote = note.GetComponent<EditorNote>();
        editorNote.noteEvent = noteEvent;
        editorNote.SetNotePosition(mouseHeight, keyOrigin.position.x);
        noteEvents.Add(noteEvent);
        

        return noteEvent;
    }

    /// <summary>
    /// Removes a note event from the editor.
    /// </summary>
    /// <param name="noteToRemove">The EditorNote to remove.</param>
    /// <param name="destroy">Flag indicating whether to destroy the GameObject.</param>
    public void RemoveNoteEvent(EditorNote noteToRemove, bool destroy)
    {
        noteEvents.Remove(noteToRemove.noteEvent);
        noteObjects.Remove(noteToRemove.gameObject);
        if (destroy) Destroy(noteToRemove.gameObject);
    }

    /// <summary>
    /// Finds and returns the note event information for the selected notes.
    /// </summary>
    /// <param name="selectedList">The list of selected EditorNoteWrapper objects.</param>
    /// <returns>A list of NoteEventInfo objects associated with the selected notes.</returns>
    public static List<NoteEventInfo> FindNoteEventInfo(HashSet<EditorNoteWrapper> selectedList)
    {
        List<NoteEventInfo> result = new();

        foreach (var wrapper in selectedList)
        {
            if (SongEditor.instance.noteObjects.Contains(wrapper.noteTransform.gameObject))
            {
                result.Add(wrapper.noteTransform.GetComponent<EditorNote>().noteEvent);
            }
        }

        return result;
    }
}
