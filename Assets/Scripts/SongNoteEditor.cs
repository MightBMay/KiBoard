using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class SongNoteEditor : MonoBehaviour
{
   /* public HashSet<EditorNoteWrapper> selectedNotes = new HashSet<EditorNoteWrapper>();
    [SerializeField] Vector2 mousePosition1;
    [SerializeField] Vector2 mousePosition2;
    [SerializeField] LayerMask groupSelectLayerMask;
    [SerializeField] bool isGroupSelecting;
    [SerializeField] bool isDragging;
    [SerializeField] bool isScaling;
    public float selectCount;
    RaycastHit2D hit;

    void Update()
    {

        OnLeftMouseDown();
        OnLeftMouseHold();
        OnLeftMouseUp();

        OnRightMouseDown();
        OnRightMouseHold();

        ScaleNote();
    }
    public void OnRightMouseDown()
    {
        if (!Input.GetMouseButtonDown(1)) return; // only run if LMB down.
        var mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition); // convert mouse position in screen space to world space.
        hit = Physics2D.Raycast(mousePositionWorld, Vector2.zero);// shoot ray from mouse position.

        if (hit) { mousePosition1 = hit.point; } // set mousepos1 if hit, otherwise return.
        else { Debug.Log("NullHit"); return; }
        UpdateSelectedNoteOffsets();



        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectNote();
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectNote();
        }
        else { SubtractNote(); }

    }
    public void OnRightMouseHold()
    {
        if (!Input.GetMouseButton(1)) return; // only run if LMB down.
        var mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition); // convert mouse position in screen space to world space.
        hit = Physics2D.Raycast(mousePositionWorld, Vector2.zero);// shoot ray from mouse position.

        if (hit) { mousePosition1 = hit.point; } // set mousepos1 if hit, otherwise return.
        else { Debug.Log("NullHit"); return; }
        UpdateSelectedNoteOffsets();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectNote();
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectNote();
        }

        else { SubtractNote(); }
    }
    public void OnLeftMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return; // only run if LMB down.
        var mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition); // convert mouse position in screen space to world space.
        hit = Physics2D.Raycast(mousePositionWorld, Vector2.zero);// shoot ray from mouse position.


        if (hit) { mousePosition1 = hit.point; Debug.Log(hit.collider.name); } // set mousepos1 if hit, otherwise return.
        else { Debug.Log("NullHit"); return; }
        UpdateSelectedNoteOffsets();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectNote();
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectNote();
        }
        else if (!isDragging)
        {
            AddNote();
        }

    }
    public void OnLeftMouseHold()
    {
        if (!Input.GetMouseButton(0)) return; // only run if LMB down.
        var mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition); // convert mouse position in screen space to world space.
        hit = Physics2D.Raycast(mousePositionWorld, Vector2.zero);// shoot ray from mouse position.

        if (hit) { mousePosition2 = hit.point; } // set mousepos1 if hit, otherwise return.
        else { Debug.Log("NullHit"); return; }



        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectNote();
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectNote();
        }
        else
        {
            DragNote();
        }

    }
    *//*   switch (operation)                                                                        old operation based system
       {
           case EditorOperation.Add:
               // AddNote();
               break;
           case EditorOperation.Subtract:
               // SubtractNote();
               break;
           case EditorOperation.Select:
               SelectNote();
               break;
           case EditorOperation.Deselect:
               DeselectNote();
               break;
           case EditorOperation.Drag:
               DragNote();
               break;
           case EditorOperation.Copy:
               CopySelection();
               break;
           case EditorOperation.Paste:
               PasteSelection();
               break;
           default:
               break;
       }
*//*
    public void OnLeftMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;

        foreach (GameObject note in SongEditor.instance.noteObjects)
        {
            UpdateStartEndTimes(note.GetComponent<EditorNote>().noteEvent,note.transform.position.y );
        }

        isDragging = false;


    }
    void AddNote()
    {
        if (hit.collider.CompareTag("KeyLane"))
        {
            DeselectAllNotes();
            int noteNumber = 1 + SpawnPiano.instance.GetIndexOfSpriteRenderer(hit.transform.parent.GetComponent<SpriteRenderer>());
            //SongEditor.instance.CreateNote(noteNumber, mousePosition1.y, mousePosition1.y + 0.5f, mousePosition1.y);
            RescaleNotesFromBPM();
        }
    }
    void SubtractNote()
    {
        if (hit.collider.TryGetComponent<EditorNote>(out var noteEvent))
        {
            DeselectAllNotes();
            selectedNotes.RemoveWhere(note => note.noteTransform == noteEvent.transform);
            SongEditor.instance.RemoveNoteEvent(noteEvent, true);
        }
    }
    void SelectNote()
    {
        if (hit.collider.CompareTag("EditorNote"))
        {
            if (!selectedNotes.Any(note => note.noteTransform == hit.transform))
            {
                var temp = new EditorNoteWrapper(hit.transform, GetOffsetFromTransform(hit.point, hit.transform));
                selectedNotes.Add(temp);
                temp.noteTransform.GetComponent<EditorNote>().SetHighlightColour(Color.red);
            }
        }

    }
    void DeselectNote()
    {
        if (hit.collider.CompareTag("EditorNote"))
        {
            HashSet<EditorNoteWrapper> noteStorage = new();
            foreach (EditorNoteWrapper note in selectedNotes)
            {
                if (note.noteTransform == hit.transform) { note.noteTransform.GetComponent<EditorNote>().ResetHighlightColour(); }
                else { noteStorage.Add(note); }
            }
            selectedNotes.Clear();
            selectedNotes = noteStorage;
        }
    }
    void DeselectAllNotes()
    {
        foreach (EditorNoteWrapper note in selectedNotes)
        {
            note.noteTransform.GetComponent<EditorNote>().ResetHighlightColour();

        }
        selectedNotes.Clear();
    }
    public void ScaleNote()
    {
        if (selectedNotes.Count > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            var scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                foreach (NoteEventInfo noteEvent in SongEditor.FindNoteEventInfo(selectedNotes))
                {

                    if (noteEvent.endTime <= noteEvent.startTime || noteEvent.endTime + Mathf.Sign(scroll) * 0.1f <= noteEvent.startTime)
                    {
                        noteEvent.endTime = noteEvent.startTime + 0.05f;
                    }
                    else
                    {

                        noteEvent.endTime += Mathf.Sign(scroll) * 0.05f;
                    }
                    RescaleNotesFromBPM();
                }
            }
        }
    }
    void DragNote()
    {
        if (hit.collider.CompareTag("EditorNote") && !isDragging)
        {
            isDragging = true;
            UpdateSelectedNoteOffsets();
            if (IsTransformInSelectedNotes(hit.transform))
            {
                var temp = new EditorNoteWrapper(hit.transform, GetOffsetFromTransform(hit.point, hit.transform));
                selectedNotes.Add(temp);
                temp.noteTransform.GetComponent<EditorNote>().SetHighlightColour(Color.red);
            }
        }
        if (selectedNotes.Count > 0 && isDragging && !Input.GetKeyDown(KeyCode.LeftShift))
        {

            Vector2 mousePosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (EditorNoteWrapper dragNote in selectedNotes)
            {
                // Calculate the snapped position
                Vector2 snappedPosition = GridSnapNote(mousePosition2 + dragNote.initialOffset);

                // Update the note's position          
                dragNote.noteTransform.position = new Vector2(snappedPosition.x - 0.05f, snappedPosition.y);
                UpdateStartEndTimes(dragNote.noteTransform.GetComponent<EditorNote>().noteEvent, snappedPosition.y);
                // idk why i need to realign it with a -0.25f shift but whatever, it works
            }
        }

    }


  

    void CopySelection()
    {

    }
    void PasteSelection()
    {

    }

    void UpdateStartEndTimes(NoteEventInfo noteEvent, float snappedYPos)
    {
        float duration = noteEvent.GetDuration();
        float newStartTime = snappedYPos - (duration / 2); // ConvertYPositionToSongTime(snappedPosition);
        noteEvent.SetStartEndTime(newStartTime, newStartTime + duration);
    }
    public float ConvertYPositionToSongTime(Vector2 position)
    {
        // 1 unit = 1 "beat" or bar on the UI
        return position.y / (GameSettings.bpm / 60);
        // y position / bpm /60 so it is bpS
    }

    public Vector2 ConvertNoteEventToNotePosition(NoteEventInfo noteEvent) // finish this to set x position as well.
    {
        var result = new Vector2(noteEvent.noteNumber * 1, noteEvent.startTime * (GameSettings.bpm / 60));
        return result;
    }
    Vector2 GetOffsetFromTransform(Vector2 point, Transform referenceTransform)
    {
        return (Vector2)referenceTransform.position - point;
    }
    bool IsTransformInSelectedNotes(Transform targetTransform)
    {
        foreach (var noteWrapper in selectedNotes)
        {
            if (noteWrapper.noteTransform == targetTransform)
            {
                return true; // Found a match
            }
        }

        return false; // No match found
    }
    public void RescaleNotesFromBPM()
    {
        float modifiedScale = GameManager.instance.modifiedNoteScale;
        foreach (GameObject g in SongEditor.instance.noteObjects)
        {
            var sr = g.GetComponent<SpriteRenderer>();
            var newSize = new Vector2(sr.size.x, Mathf.Clamp(((
                g.GetComponent<EditorNote>().noteEvent.endTime -
                g.GetComponent<EditorNote>().noteEvent.startTime)), 0.5f, Mathf.Infinity));

            sr.size = newSize;
           //sr.GetComponent<EditorNote>().SetShadowSize(newSize.y);
        }

    }

    void UpdateSelectedNoteOffsets()
    {
        foreach (EditorNoteWrapper dragNote in selectedNotes)
        {
            dragNote.initialOffset = GetOffsetFromTransform(hit.point, dragNote.noteTransform); // assign all offset values for clicking and dragging.
        }
    }
    public static Vector2 GridSnapNote(Vector2 input, float increment = 0.204545f)
    {
        float x = Mathf.Round(input.x / increment) * increment;

        return new Vector2(x-0.095f, input.y);
    }

*/
}





[System.Serializable]
public class EditorNoteWrapper
{
    public Transform noteTransform;
    public Vector2 initialOffset; // Store the initial offset here

    public EditorNoteWrapper(Transform transform, Vector2 offset)
    {
        noteTransform = transform;
        initialOffset = offset;
    }
}
