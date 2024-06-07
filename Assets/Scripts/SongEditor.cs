using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

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

    public Color noteColour;
    public Color selectedNoteColour;

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

    public readonly float smallestAllowedNote = 1 / 16f;

    /// <summary>
    /// list of editornotes.
    /// </summary>
    [SerializeField] List<EditorNote> editorNotes = new();

    internal HashSet<EditorNote> selectedNotes = new();

    /// <summary>
    /// EditorActions for the left, middle and right mouse buttons respectively.<br/>
    /// Can be bound by calling <see cref="InitializeAction(string, sbyte)"/> with a string corresponding to the name of the editor action, and a sByte containing the number mouse button.
    /// </summary>
    public EditorAction leftAction, middleAction, rightAction;


    //Instances of the different editor actions so i don't make a new one every time.
    AddNote addNote = new();
    RemoveNotes removeNotes = new();
    SelectNotes selectNotes = new();
    ScaleNotes scaleNotes = new();



    Button currentlySelectedButton;
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
        leftAction?.HandleInput();
        middleAction?.HandleInput();
        rightAction?.HandleInput();
    }


    /// <summary>
    /// Takes a raycast hit and spawns an editorNorePrefab.
    /// </summary>
    /// <param name="hit"> raycast2dhit information.</param>
    public void CreateNote(RaycastHit2D hit)
    {
        Transform note = Instantiate(editorNotePrefab, noteHolder).transform;// create note
        int noteNum = Mathf.RoundToInt(hit.point.x); // round notes position to get the key number.
        float height = hit.point.y;//get height of the note\
        EditorNote editorNote = note.GetComponent<EditorNote>();
        Vector2 snappeedPos = SnapNote(new Vector2(noteNum, height));
        VSnapNote(note);// scale note based on VSnap.
        float halfNoteHeight = note.localScale.y / 2;
        snappeedPos.y += halfNoteHeight;
        note.position = snappeedPos; // round the notes position to nearest fraction of a beat.
        editorNote.UpdateNoteEvent(noteNum, snappeedPos.y - 2f - halfNoteHeight, snappeedPos.y - 2f + halfNoteHeight); // set noteEvent data.

        editorNotes.Add(editorNote); // add to list of noteEvents.

    }
    /// <summary>
    /// scales a note based on vsnap 
    /// </summary>
    /// <param name="note"> transform to have scale snapped.</param>
    public void VSnapNote(Transform note)
    {
        if (scaleNoteToVSnap)
        {
            if (defaultNoteScale.y > vSnap)
            {
                note.localScale = new(defaultNoteScale.y, Utility.RoundToFraction(defaultNoteScale.y, vSnap));
            }
            else
            {
                note.localScale = new(defaultNoteScale.y, 15 / vSnap);
            }
        }

    }


    /// <summary>
    /// removes an editornote from the editorNotes list and then destroys the gameObject it is attatched to.
    /// </summary>
    /// <param name="note">note to be removed.</param>
    public void RemoveNote(EditorNote note)
    {
        editorNotes.Remove(note);
        Destroy(note.gameObject);
    }

    /// <summary>
    /// Spawns the piano roll
    /// </summary>
    public void InitializePianoRoll()
    {
        for (int i = 0; i < 88; i++) // for each of the 88 keys:
        {
            Transform trans = Instantiate(keyPrefab, transform).transform; // spawn key and get position
            trans.position = new(i, 0, 0); // set key position
            trans.localScale = baseKeyScale; // set key scale
            trans.GetComponent<SpriteRenderer>().color = GetKeyColour(i); //set key colour
            Transform lane = Instantiate(keyPrefab, trans).transform; // create keyLane for key
            lane.GetComponent<SpriteRenderer>().color = i % 2 == 0 ? keyLaneColour1 : keyLaneColour2; // key lane colour that alternates
            lane.localScale = keyLaneScale; // set keyLane Scale
            lane.position += new Vector3(0f, (keyLaneScale.y * baseKeyScale.y / 2) - 2.5f, 1f); // set key lane position.
            lane.gameObject.tag = "KeyLane";

            keyLanes.Add(i, lane.gameObject); // add keylane to dictionary with X position as the key.
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
    Vector2 SnapNote(Vector2 pos)
    {
        if (vSnap <= 0) { return pos; } // set to 0 for no snapping.
        else { return new Vector2(pos.x, Utility.RoundToFraction(pos.y - 5f, vSnap) + 2.5f); } //otherwise round y to nearest fraction given ( subtraction and addition for some offsets).
    }


    /// <summary>
    /// Called in the editor ui buttons. When clicked it will grey out a selected button and un grey out the previously selected one.
    /// </summary>
    /// <param name="button"></param>
    public void SelectButton(Button button)
    {
        if (currentlySelectedButton != null)
        {
            currentlySelectedButton.interactable = true;
        }

        button.interactable = false;
        currentlySelectedButton = button;
        leftAction = InitializeAction(button.gameObject.name, 0);
        rightAction = InitializeAction("scale", 1);
        middleAction = InitializeAction("select", 2);
    }

    public EditorAction InitializeAction(string actionName, sbyte mouseNumber)
    {
        switch (actionName.ToLower()) // convert actionName to lower and set up the respective action for use with mousebutton # mouseNumber
        {
            case "add":
                return addNote.SetEditorAction(mouseNumber);
            case "remove":
                return removeNotes.SetEditorAction(mouseNumber);
            case "select":
                return selectNotes.SetEditorAction(mouseNumber);
            case "scale":
                return scaleNotes.SetEditorAction(mouseNumber);
            default:
                return null;

        }
    }

    public void ClearSelectedNotes()
    {
        foreach (EditorNote note in selectedNotes)
        {
            note.SetColour(noteColour);
        }
        selectedNotes.Clear();
    }
}
/// <summary>
/// Base EditorAction for other more specific actions to inherit from.
/// </summary>
public class EditorAction
{
    protected sbyte mouseButton;
    protected RaycastHit2D hit;

    /// <summary>
    /// Returns a pre existing editoraction and allows you to set the button used to activate it at the same time.
    /// </summary>
    /// <param name="mouseNum">new mouse number assigned to the editor action.</param>
    /// <returns> editor action.</returns>
    public EditorAction SetEditorAction(sbyte mouseNum)
    {
        if (mouseNum < 0 || mouseNum >= 3) { mouseButton = -1; }
        mouseButton = mouseNum;

        return this;
    }
    public virtual void HandleInput()
    {

        if (GetMouseRaycast())
        {
            Down();
            Hold();
            Up();
        }
        ScrollWheel();
    }
    /// <summary>
    /// Called on MouseButtonDown()
    /// </summary>
    protected virtual void Down()
    {

    }
    /// <summary>
    /// Called on MouseButton() (excludes first frame pressed)
    /// </summary>
    protected virtual void Hold()
    {
    }
    /// <summary>
    /// Called on MouseButtonUp()
    /// </summary>
    protected virtual void Up()
    {

    }
    protected virtual void ScrollWheel()
    {

    }

    /// <summary>
    /// Sends raycast from Camera.main to mouse position and assigns the raycasthit2d to <see cref="hit"/>.
    /// </summary>
    /// <returns>Whether or not the raycast hit an object.</returns>
    protected bool GetMouseRaycast()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit;
    }

    protected bool CheckDown() { return !Input.GetMouseButtonDown(mouseButton); }// return true if mouse button not down on this frame.
    protected bool CheckHold() { return !Input.GetMouseButton(mouseButton) && CheckDown(); }// return true if mouse button not held and if checkdown()
    protected bool CheckUp() { return !Input.GetMouseButtonUp(mouseButton); } //retun true if mouse button not up on this frame.
}
/// <summary>
/// <see cref="EditorAction"/> for creating new notes.
/// </summary>
public class AddNote : EditorAction
{
    protected override void Down()
    {
        // if the mouse button the EditorAction was on was not pressed this frame, and wasn't on a key lane.
        if (CheckDown() || (hit && !hit.collider.CompareTag("KeyLane"))) return;
        CreateNote();

    }
    protected override void Hold()
    {
        // works fine, but if ur mouse ends up placing a note while not on a note it makes one every single frame.
        //if (CheckHold()) return;
        //CreateNote();
    }
    /// <summary>
    /// Clear selected notes, then create a note at the hit point.
    /// </summary>
    void CreateNote()
    {
        SongEditor.instance?.ClearSelectedNotes();// when other EditorActions are taken, clear the selected notes.
        SongEditor.instance?.CreateNote(hit); // create note.
    }
}

/// <summary>
/// <see cref="EditorAction"/> for removing and destroying notes.
/// </summary>
public class RemoveNotes : EditorAction
{
    protected override void Down()
    {
        if (CheckDown()) return;
        RemoveNoteIfHit();
    }
    protected override void Hold()
    {
        if (CheckHold()) return;
        RemoveNoteIfHit();
    }

    void RemoveNoteIfHit()
    {
        SongEditor.instance?.ClearSelectedNotes();// when other EditorActions are taken, clear the selected notes.
        if (hit.collider.TryGetComponent(out EditorNote note))
        {
            SongEditor.instance.RemoveNote(note);
        }
    }
}
/// <summary>
/// <see cref="EditorAction"/> for selecting notes to be modified by other editor actions.
/// </summary>
public class SelectNotes : EditorAction
{
    public override void HandleInput()
    {
        if (GetMouseRaycast())
        {
            Down();
            Hold();
            Up();
        }
        ScrollWheel();
    }
    protected override void Down()
    {
        if (CheckDown()) return;
        SelectNote();
    }
    protected override void Hold()
    {
        if (CheckHold()) return;
        SelectNote();
    }
    /// <summary>
    /// Selects notes based on <see cref="EditorAction.GetMouseRaycast"/>'s raycasthit2d.
    /// </summary>
    void SelectNote()
    {
        if (hit.collider.TryGetComponent(out EditorNote note)) // if editor note is hit
        {
            HashSet<EditorNote> editorNotes = SongEditor.instance.selectedNotes; // get hashset of all created notes

            if (Input.GetKey(KeyCode.LeftShift)) //if left shift held remove the note from selectedNotes and reset their colour.
            {

                note.SetColour(SongEditor.instance.noteColour);
                editorNotes.Remove(note);
            }
            else // if left shift not held, add notes to selection and change their colour.
            {
                if (editorNotes != null)
                {
                    editorNotes.Add(note);
                    note.SetColour(SongEditor.instance.selectedNoteColour);
                }
            }

        }

    }
}

public class ScaleNotes : EditorAction
{

    protected override void ScrollWheel()
    {
        sbyte scrollDir = (sbyte)(Input.mouseScrollDelta.y == 0 ? 0 : Mathf.Sign(Input.mouseScrollDelta.y)); // if scrolldelta not zero, get the sign.
        float minSize = (15 / SongEditor.instance.vSnap);
        float scaleStep = scrollDir * minSize;
        Debug.Log(minSize);
        if (scrollDir == 0) return; // return if not scrolling.

        foreach (EditorNote note in SongEditor.instance.selectedNotes)
        {
           


            float newY = note.transform.localScale.y +scaleStep;
            float clampedY = Mathf.Clamp(newY, minSize, Mathf.Infinity);
            if (clampedY-minSize > minSize) { note.transform.position += Vector3.up * scaleStep / 2; Debug.Log(clampedY); }

            note.transform.localScale = new Vector3(note.transform.localScale.x, clampedY, note.transform.localScale.z); // assign new scale
            var updatedScale = note.transform.localScale;
            note.UpdateNoteEvent(note.transform.position.y - (updatedScale.y/2), note.transform.position.y + (updatedScale.y / 2) ); // update timings.
        }
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