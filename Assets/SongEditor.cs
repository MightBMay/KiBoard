using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SongEditor : MonoBehaviour
{
    public static SongEditor instance;
    public GameObject editorNotePrefab;
    public List<GameObject> noteObjects;
    public List<NoteEventInfo> noteEvents;
    public Transform noteHolder, keyOrigin;

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
        DebugStuff();
    }
    void DebugStuff()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartSongTest();
        }

    }
    public void StartSongTest()
    {
        StartCoroutine(MidiInput.instance.StartSong(noteEvents));
    }


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

    public void RemoveNoteEvent(EditorNote noteToRemove, bool destroy)
    {

        noteEvents.Remove(noteToRemove.noteEvent);
        noteObjects.Remove(noteToRemove.gameObject);
        if (destroy) Destroy(noteToRemove.gameObject);

    }


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





