using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;

/// <summary>
/// Manages the song editor functionality.
/// </summary>
public class SongEditor : MonoBehaviour
{
    // Start is called before the first frame update
    public static SongEditor instance;

    /// <summary>
    /// Dictionary of intiger X positions  and key lane objects for easy detection and positioning.
    /// </summary>
    public Dictionary<int, GameObject> keyLanes = new();
    /// <summary>
    /// parent of all notes spawned by player.
    /// </summary>
    [SerializeField] Transform noteHolder;
    [SerializeField] GameObject editorNotePrefab;
    Camera cam;
    [SerializeField] GameObject keyPrefab;
    [SerializeField] Color keyLaneColour1, keyLaneColour2;
    /// <summary>
    /// size of key lanes
    /// </summary>
    [SerializeField] Vector3 keyLaneScale = new Vector3(1, 20, 1);
    /// <summary>
    /// size of piano keys.
    /// </summary>
    [SerializeField] Vector3 baseKeyScale = new Vector3(.8f, 5, 1);
    [SerializeField] Vector2 defaultNoteScale = new(1, 1);

    /// <summary>
    /// every 15 units in unity is one beat.
    /// </summary>
    public float heightMultiplier = 15;

    /// <summary>
    /// how many subdivisions for vertical note snapping.
    /// </summary>
    public float vSnap = 0;
    /// <summary>
    /// should new notes automatically scale to the size indicated by vSnap.
    /// </summary>
    public bool scaleNoteToVSnap;


    #region Ui References
    Button currentlySelectedButton;
    #endregion
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }
    void Start()
    {
        cam = Camera.main;

        InitializePianoRoll();
    }
    private void Update()
    {
        HandleMouseInput();
    }


    public void HandleMouseInput()
    {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return; // change this to account for a isdragging variable when we get there.       

        Right();
        Left();
        Middle();

        void Left()
        {
            Down();
            Hold();
            Up();
            void Down()
            {

                if (!Input.GetMouseButtonDown(0)) { return; }
                AddNote(hit);
            }


            void Hold()
            {
                // if you are holding LMB but it isnt the first frame you pressed it:
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) { return; }

                //hold logic
            }


            void Up()
            {
                if (!Input.GetMouseButtonDown(0)) { return; }
            }
        }

        void Middle()
        {
            Down();
            Hold();
            Up();


            void Down()
            {

                if (!Input.GetMouseButtonDown(0)) { return; }
            }


            void Hold()
            {
                // if you are holding LMB but it isnt the first frame you pressed it:
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) { return; }

                //hold logic
            }


            void Up()
            {
                if (!Input.GetMouseButtonDown(0)) { return; }
            }
        }

        void Right()
        {
            Down();
            Hold();
            Up();
            

            void Down()
            {

                if (!Input.GetMouseButtonDown(1)) { return; }
                test();
                void test()
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Debug.Log(Utility.RoundToFraction(i, vSnap));
                    }
                }

            }


            void Hold()
            {
                // if you are holding LMB but it isnt the first frame you pressed it:
                if (Input.GetMouseButton(1) && !Input.GetMouseButtonDown(1)) { return; }

                //hold logic
            }


            void Up()
            {
                if (!Input.GetMouseButtonDown(1)) { return; }
            }
        }

        // make one for scroll wheel.
    }

    /// <summary>
    /// Takes a raycast hit and spawns an editorNorePrefab.
    /// </summary>
    /// <param name="hit"> raycast2dhit information.</param>
    void AddNote(RaycastHit2D hit)
    {
        Transform note = Instantiate(editorNotePrefab, noteHolder).transform;// create note
        int noteNum = Mathf.RoundToInt(hit.point.x); // round notes position to get the key number.
        float height = hit.point.y;//get height of the note\
        EditorNote editorNote = note.GetComponent<EditorNote>();
        if (scaleNoteToVSnap)
        {
            if (defaultNoteScale.y > vSnap)
            {
                note.localScale = new(defaultNoteScale.y, Utility.RoundToFraction(defaultNoteScale.y, vSnap));
            }
            else
            {
                note.localScale = new(defaultNoteScale.y, ScaleToBeat(vSnap, vSnap)) ;
            }
        }
        Vector2 snappeedPos = SnapNote(new Vector2(noteNum, height));
        snappeedPos.y += (note.localScale.y / 2);
        note.position = snappeedPos; // round the notes position to nearest fraction of a beat.
        editorNote.UpdateNoteEvent(noteNum, snappeedPos.y - 2f - (defaultNoteScale.y / 2), snappeedPos.y - 2f + (defaultNoteScale.y / 2));

    }
    /// <summary>
    /// Spawns the piano roll
    /// </summary>
    public void InitializePianoRoll()
    {
        for (int i = 0; i < 88; i++)
        {
            Transform trans = Instantiate(keyPrefab, transform).transform;
            trans.position = new(i, 0, 0);
            trans.localScale = baseKeyScale;
            trans.GetComponent<SpriteRenderer>().color = GetKeyColour(i);
            Transform lane = Instantiate(keyPrefab, trans).transform;
            lane.GetComponent<SpriteRenderer>().color = i % 2 == 0 ? keyLaneColour1 : keyLaneColour2;
            lane.localScale = keyLaneScale;
            lane.position += new Vector3(0f, (keyLaneScale.y * baseKeyScale.y / 2) - 2.5f, 1f);
            lane.gameObject.tag = "KeyLane";

            keyLanes.Add(i, lane.gameObject);
        }
        Color GetKeyColour(int i)
        {
            int value = i % 12;
            if (value == 0 || value == 2 || value == 3 || value == 5 || value == 7 || value == 9 || value == 11) return Color.white;
            else { return Color.black; }
        }
    }


    /// <summary>
    /// Rounds a vector2's values to the nearest fraction of 1/Vsnap
    /// </summary>
    /// <param name="pos">Vector to snap</param>
    /// <returns>vector with fraction-rounded values.</returns>
    public Vector2 SnapNote(Vector2 pos)
    {
        if (vSnap <= 0) { return pos; }
        else { return new Vector2(pos.x, Utility.RoundToFraction(pos.y-5f, vSnap) + 2.5f); }
    }
    public  float ScaleToBeat(float num, float unitsPerBeat = 15f)
    {
        return 15/ num;
    }


    public void SelectButton(Button button)
    {
        if(currentlySelectedButton != null)
        {
            currentlySelectedButton.interactable = true;
        }

        button.interactable = false;
        currentlySelectedButton = button;
    }

}




/*
public class oldSongEditor : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the SongEditor class.
    /// </summary>
    public static SongEditor instance;

    /// <summary>
    /// Prefab for the editor note object.
    /// </summary>
    public GameObject editorNotePrefab;

    public GameObject[] keyLanes = new GameObject[88];

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
        //        LoadSongAsEditorNotes();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Debugging method to perform certain actions based on input.
    /// </summary>
    private void DebugStuff()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Start testing the song
            //SongTestCoroutine=  StartCoroutine(StartSongTest());
        }
    }

    #region Mouse inputs

    public void HandleMouseInput()
    {
        RightMouse();
        MiddleMouse();
        LeftMouse();
    }
    public void RightMouse()
    {
        // Create New note
        GameObject collider = RaycastFromMouse(out RaycastHit hit); // raycast for position and key lane hit.
        if (collider.CompareTag("KeyLane"))
        {
            int noteNum;
            try { noteNum = Array.IndexOf(keyLanes, collider.gameObject) + 1; } // get the x position that is equal to the index+1.
            catch (Exception e) { Debug.Log("Error Finding index of key lane. Exception:\n" + e); return; }
            CreateNote(noteNum, hit.point.y);
        }

    }

    public void MiddleMouse()
    {

    }

    public void LeftMouse()
    {

    }

    public GameObject RaycastFromMouse(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    #endregion


    #region Other
    void LoadSongAsEditorNotes()
    {
        NoteEventDataWrapper notes = MidiInput.instance.GetNoteEventWrapperFromSelectedSong();
        GameSettings.bpm = notes.BPM;
        foreach (var note in notes.NoteEvents)
        {
            note.noteNumber -= 20;
        }
        foreach (var note in notes.NoteEvents)
        {
            //  if(GameManager.CheckSpawnNote(note))CreateNote(note.noteNumber, note.startTime, note.endTime, (note.startTime+note.endTime)/2 );
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
        yield return new WaitUntil(() => IsTestDone());
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
    public NoteEventInfo CreateNote(int noteNumber, float mouseHeight)
    {
        float startTime = 0, endTime = 0; // FIGURE OUT HOW TO TRANSLATE NOTE HEIGHT TO TIME BASED OFF BPM.
        GameObject note = Instantiate(editorNotePrefab, noteHolder);
        noteObjects.Add(note);
        var noteEvent = new NoteEventInfo(noteNumber, startTime, endTime);
        var editorNote = note.GetComponent<EditorNote>();
        editorNote.noteEvent = noteEvent;
        editorNote.SetNotePosition(mouseHeight);
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

    #endregion Other

    
}*/